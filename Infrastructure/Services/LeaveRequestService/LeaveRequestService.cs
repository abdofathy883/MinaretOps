using AutoMapper;
using Core.DTOs.Attendance;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.LeaveRequestService
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<LeaveRequestService> logger;
        public LeaveRequestService(
            MinaretOpsDbContext dbContext,
            IEmailService _emailService,
            IMapper _mapper,
            UserManager<ApplicationUser> _userManager,
            ILogger<LeaveRequestService> _logger
            )
        {
            context = dbContext;
            emailService = _emailService;
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

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                request.Status = newStatus;
                request.ActionDate = DateTime.UtcNow;
                context.Update(request);
                await context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(request.Employee.Email))
                {
                    Dictionary<string, string> replacements = new()
                    {
                        { "EmployeeName", $"{request.Employee.FirstName} {request.Employee.LastName}" },
                        { "FromDate", $"{request.FromDate}" },
                        { "ToDate", $"{request.ToDate}" },
                    };

                    await emailService.SendEmailWithTemplateAsync(request.Employee.Email, "Leave Request Updates", "LeaveRequestUpdates", replacements);
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
                var request = new LeaveRequest
                {
                    EmployeeId = emp.Id,
                    FromDate = leaveRequestDTO.FromDate,
                    ToDate = leaveRequestDTO.ToDate,
                    Status = LeaveStatus.Pending,
                    ActionDate = DateTime.UtcNow
                };
                await context.AddAsync(request);
                await context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(emp.Email))
                {
                    Dictionary<string, string> replacements = new()
                    {
                        { "EmployeeName", $"{emp.FirstName} {emp.LastName}" },
                        { "RequestFromDate", $"{request.FromDate}" },
                        { "RequestToDate", $"{request.ToDate}" },
                        { "RequestStatus", $"{request.Status.ToString()}" }
                    };
                    await emailService.SendEmailWithTemplateAsync(emp.Email, "New Leave Request", "NewLeaveRequest", replacements);
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
