using AutoMapper;
using Core.DTOs.Attendance;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.MediaUploads;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services.LeaveRequestService
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly MinaretOpsDbContext context;
        private readonly MediaUploadService mediaUploadService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<LeaveRequestService> logger;
        public LeaveRequestService(
            MinaretOpsDbContext dbContext,
            MediaUploadService _mediaUploadService,
            IMapper _mapper,
            UserManager<ApplicationUser> _userManager,
            ILogger<LeaveRequestService> _logger
            )
        {
            context = dbContext;
            mediaUploadService = _mediaUploadService;
            mapper = _mapper;
            userManager = _userManager;
            logger = _logger;
        }
        public async Task<LeaveRequestDTO> ChangeLeaveRequestStatusByAdminAsync(string adminId, int requestId, LeaveStatus newStatus)
        {
            var admin = await GetUserOrThrow(adminId);
            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                throw new UnauthorizedAccessException("هذا المستخدم غير مصرح له بهذا الاجراء");

            var request = await context.LeaveRequests
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw new InvalidObjectException($"لم يتم العثور على طلب الاجازة بهذا المعرف {requestId}");

            var emp = await GetUserOrThrow(request.EmployeeId);

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                request.Status = newStatus;
                request.ActionDate = DateTime.UtcNow;
                context.Update(request);
                await context.SaveChangesAsync();

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
                            { "FromDate", $"{request.FromDate}" },
                            { "ToDate", $"{request.ToDate}" },
                            { "RequestStatus", $"{request.Status.ToString()}" },
                            { "LeaveType", request.Type.ToString() },
                            { "RequestReason", request.Reason },
                            { "TimeStamp", $"{request.RequestDate.ToString("yyyy-MM-dd")}" }
                        }
                    };

                    var emailOutBox = new Outbox
                    {
                        OpType = OutboxTypes.Email,
                        OpTitle = "Send Leave Request Updates Email",
                        PayLoad = JsonSerializer.Serialize(emailPayload)
                    };

                    await context.OutboxMessages.AddAsync(emailOutBox);
                    await context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
                return mapper.Map<LeaveRequestDTO>(request);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
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
            var emp = await GetUserOrThrow(employeeId);

            var requests = await context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId)
                .ToListAsync();

            return mapper.Map<List<LeaveRequestDTO>>(requests);
        }
        public async Task<LeaveRequestDTO> SubmitLeaveRequest(CreateLeaveRequestDTO leaveRequestDTO)
        {
            var emp = await GetUserOrThrow(leaveRequestDTO.EmployeeId);

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
                await context.SaveChangesAsync();

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

                    var emailOutBox = new Outbox
                    {
                        OpType = OutboxTypes.Email,
                        OpTitle = "Send New Leave Request Email",
                        PayLoad = JsonSerializer.Serialize(emailPayload)
                    };

                    await context.OutboxMessages.AddAsync(emailOutBox);
                    await context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return mapper.Map<LeaveRequestDTO>(request);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new NotImplementedOperationException(ex.Message);
            }
        }
        private async Task<ApplicationUser> GetUserOrThrow(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new InvalidObjectException($"لم يتم العثور على موظف بهذا المعرف {userId}");
            return user;
        }
    }
}
