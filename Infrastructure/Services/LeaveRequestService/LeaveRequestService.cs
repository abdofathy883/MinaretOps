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
                throw new UnauthorizedAccessException("User is not authorized to perform this action.");

            var request = await context.LeaveRequests
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw new InvalidObjectException($"Leave request with Id {requestId} not found.");

            request.Status = newStatus;
            request.ActionDate = DateTime.UtcNow;
            context.Update(request);
            await context.SaveChangesAsync();
            //return mapper.Map<LeaveRequestDTO>(request);
            var mappedResult = mapper.Map<LeaveRequestDTO>(request);
            if (mappedResult == null)
                throw new InvalidOperationException("Failed to map leave request to DTO");

            return mappedResult;
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
            if (leaveRequestDTO is null)
                throw new InvalidObjectException("");

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
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidObjectException($"User with Id {userId} not found");
            return user;
        }
    }
}
