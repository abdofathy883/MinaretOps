using AutoMapper;
using Core.DTOs.Attendance;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.MediaUploads;
using Infrastructure.Services.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.LeaveRequestService
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly MediaUploadService mediaUploadService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public LeaveRequestService(
            MinaretOpsDbContext dbContext,
            TaskHelperService _helperService,
            MediaUploadService _mediaUploadService,
            IMapper _mapper,
            UserManager<ApplicationUser> _userManager
            )
        {
            context = dbContext;
            helperService = _helperService;
            mediaUploadService = _mediaUploadService;
            mapper = _mapper;
            userManager = _userManager;
        }
        public async Task<LeaveRequestDTO> ChangeLeaveRequestStatusByAdminAsync(string adminId, int requestId, LeaveStatus newStatus)
        {
            var admin = await helperService.GetUserOrThrow(adminId)
                ?? throw new InvalidObjectException("المستخدم غير موجود");

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                throw new UnauthorizedAccessException("هذا المستخدم غير مصرح له بهذا الاجراء");

            var request = await context.LeaveRequests
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw new InvalidObjectException($"لم يتم العثور على طلب الاجازة بهذا المعرف {requestId}");

            var emp = await helperService.GetUserOrThrow(request.EmployeeId)
                ?? throw new InvalidObjectException("الموظف غير موجود");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                request.Status = newStatus;
                request.ActionDate = DateTime.UtcNow;
                context.Update(request);

                if (!string.IsNullOrEmpty(emp.Email))
                {
                    var emailPayload = new
                    {
                        To = emp.Email,
                        Subject = "Leave Request Updates",
                        Template = "LeaveRequestUpdates",
                        Replacements = new Dictionary<string, string>
                        {
                            { "EmployeeName", $"{request.Employee.FirstName} {request.Employee.LastName}" },
                            { "RequestFromDate", $"{request.FromDate}" },
                            { "RequestToDate", $"{request.ToDate}" },
                            { "RequestStatus", $"{request.Status.ToString()}" },
                            { "LeaveType", request.Type.ToString() },
                            { "RequestReason", request.Reason },
                            { "TimeStamp", $"{request.RequestDate.ToString("yyyy-MM-dd")}" }
                        }
                    };
                    await helperService.AddOutboxAsync(OutboxTypes.Email, "Leave Request Updates Email", emailPayload);
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<LeaveRequestDTO>(request);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<LeaveRequestDTO>> GetAllLeaveRequests()
        {
            var requests = await context.LeaveRequests
                .Include(r => r.Employee)
                .ToListAsync();
            return mapper.Map<List<LeaveRequestDTO>>(requests);
        }
        public async Task<List<LeaveRequestDTO>> GetLeaveRequestsByEmployee(string employeeId)
        {
            var emp = await helperService.GetUserOrThrow(employeeId)
                ?? throw new InvalidObjectException("الموظف غير موجود");

            var requests = await context.LeaveRequests
                .Where(r => r.EmployeeId == emp.Id)
                .ToListAsync();

            return mapper.Map<List<LeaveRequestDTO>>(requests);
        }
        public async Task<LeaveRequestDTO> SubmitLeaveRequest(CreateLeaveRequestDTO leaveRequestDTO)
        {
            var emp = await helperService.GetUserOrThrow(leaveRequestDTO.EmployeeId)
                ?? throw new InvalidObjectException("الموظف غير موجود");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var fileUrl = string.Empty;
                if (leaveRequestDTO.ProofFile != null)
                {
                    var contentType = leaveRequestDTO.ProofFile.ContentType.ToLower();

                    if (contentType.StartsWith("image/"))
                    {
                        var uploaded = await mediaUploadService.UploadImageWithPath(leaveRequestDTO.ProofFile, $"{emp.FirstName} {emp.LastName}");
                        fileUrl = uploaded.Url;
                    }
                    else
                    {
                        var uploaded = await mediaUploadService.UploadPDFFile(leaveRequestDTO.ProofFile, $"{emp.FirstName} {emp.LastName}");
                        fileUrl = uploaded;
                    }
                }
                var request = new LeaveRequest
                {
                    EmployeeId = emp.Id,
                    FromDate = leaveRequestDTO.FromDate,
                    ToDate = leaveRequestDTO.ToDate,
                    Type = leaveRequestDTO.Type,
                    Reason = leaveRequestDTO.Reason,
                    Status = LeaveStatus.Pending,
                    ActionDate = DateTime.UtcNow,
                    RequestDate = DateTime.UtcNow,
                    ProofFile = fileUrl
                };
                await context.LeaveRequests.AddAsync(request);

                if (!string.IsNullOrEmpty(emp.Email))
                {
                    var emailPayload = new
                    {
                        To = emp.Email,
                        Subject = "New Leave Request",
                        Template = "NewLeaveRequest",
                        Replacements = new Dictionary<string, string>
                        {
                            { "EmployeeName", $"{emp.FirstName} {emp.LastName}" },
                            { "RequestFromDate", $"{request.FromDate}" },
                            { "RequestToDate", $"{request.ToDate}" },
                            { "RequestStatus", $"{request.Status.ToString()}" },
                            { "LeaveType", request.Type.ToString() },
                            { "RequestReason", request.Reason },
                            { "RequestFile", request.ProofFile },
                            { "TimeStamp", $"{request.RequestDate.ToString("yyyy-MM-dd")}" }
                        }
                    };
                    await helperService.AddOutboxAsync(OutboxTypes.Email, "New Leave Request Email", emailPayload);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<LeaveRequestDTO>(request);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
