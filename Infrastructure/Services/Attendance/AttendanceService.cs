using AutoMapper;
using Core.DTOs.Attendance;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Attendance
{
    public class AttendanceService : IAttendanceService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public AttendanceService(
            MinaretOpsDbContext dbContext,
            IEmailService _emailService,
            IMapper _mapper,
            UserManager<ApplicationUser> _userManager
            )
        {
            context = dbContext;
            emailService = _emailService;
            mapper = _mapper;
            userManager = _userManager;
        }

        public async Task<AttendanceRecordDTO> ChangeAttendanceStatusByAdminAsync(string adminId, int recordId, AttendanceStatus newStatus)
        {
            var admin = await GetUserOrThrow(adminId);
            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                throw new UnauthorizedAccessException("User is not authorized to perform this action.");

            var record = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.Id == recordId)
                ?? throw new InvalidObjectException($"Attendance record with Id {recordId} not found.");                

            record.Status = newStatus;
            context.Update(record);
            await context.SaveChangesAsync();
            return mapper.Map<AttendanceRecordDTO>(record);
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

        public async Task<List<AttendanceRecordDTO>> GetAllAttendanceRecords()
        {
            var records = await context.AttendanceRecords
                .Include(r => r.Employee)
                .ToListAsync();
            return mapper.Map<List<AttendanceRecordDTO>>(records);
        }

        public async Task<List<LeaveRequestDTO>> GetAllLeaveRequests()
        {
            var requests = await context.LeaveRequests
                .Include(r => r.Employee)
                .ToListAsync();
            return mapper.Map<List<LeaveRequestDTO>>(requests);
        }

        public async Task<List<AttendanceRecordDTO>> GetAttendanceRecordsByEmployee(string employeeId)
        {
            var emp = await GetUserOrThrow(employeeId);

            var records = await context.AttendanceRecords
                .Where(r => r.EmployeeId == employeeId)
                .ToListAsync();

            return mapper.Map<List<AttendanceRecordDTO>>(records);
        }

        public async Task<List<LeaveRequestDTO>> GetLeaveRequestsByEmployee(string employeeId)
        {
            var emp = await GetUserOrThrow(employeeId);

            var requests = await context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId)
                .ToListAsync();

            return mapper.Map<List<LeaveRequestDTO>>(requests);
        }

        public async Task<AttendanceRecordDTO> GetTodayAttendanceForEmployeeAsync(string empId)
        {
            var emp = await GetUserOrThrow(empId);

            var attendanceRecords = await context.AttendanceRecords
                .Where(a => a.CheckInTime >= DateTime.UtcNow.Date && a.EmployeeId == empId)
                .FirstOrDefaultAsync();

            return mapper.Map<AttendanceRecordDTO>(attendanceRecords);
        }

        public async Task<AttendanceRecordDTO> NewAttendanceRecord(CreateAttendanceRecordDTO recordDTO)
        {
            if (recordDTO is null)
                throw new InvalidObjectException("");

            var user = await GetUserOrThrow(recordDTO.EmployeeId);

            var existingRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == recordDTO.EmployeeId && r.CheckInTime.Date == DateTime.UtcNow.Date);

            if (existingRecord != null)
                throw new InvalidObjectException("Attendance record for today already exists.");

            var otherEmpsWithSameIp = await context.AttendanceRecords
                .Where(r => r.IpAddress == recordDTO.IpAddress && 
                r.EmployeeId != recordDTO.EmployeeId && 
                r.CheckInTime.Date == DateTime.UtcNow.Date)
                .Include(r => r.Employee)
                .ToListAsync();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var attendanceRecord = new AttendanceRecord
                {
                    EmployeeId = recordDTO.EmployeeId,
                    CheckInTime = DateTime.UtcNow,
                    DeviceId = recordDTO.DeviceId,
                    IpAddress = recordDTO.IpAddress,
                    Status = recordDTO.Status
                };
                await context.AddAsync(attendanceRecord);
                await context.SaveChangesAsync();

                if (otherEmpsWithSameIp.Any())
                {
                    var otherEmployeesNames = string.Join(", ",
                        otherEmpsWithSameIp.Select(r => $"{r.Employee.FirstName} {r.Employee.LastName}"));

                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        { "CurrentEmpName", $"{user.FirstName} {user.LastName}" },
                        { "CurrentEmpEmail", user.Email ?? string.Empty },
                        { "CurrentEmpId", user.Id },
                        { "SuspiciousIp", recordDTO.IpAddress },
                        { "CheckInTime", attendanceRecord.CheckInTime.ToString("u") },
                        { "DeviceId", recordDTO.DeviceId },
                        { "OtherEmployees", otherEmployeesNames },
                        { "TotalEmployeesOnIp", (otherEmpsWithSameIp.Count + 1).ToString() },
                    };

                    await emailService.SendEmailWithTemplateAsync("zminaretagency@gmail.com", "Attendance Alert : Multiple Employees Using Same Device", "AttendanceAlert", replacements);
                }

                await transaction.CommitAsync();
                return mapper.Map<AttendanceRecordDTO>(attendanceRecord);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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
                    Date = leaveRequestDTO.Date,
                    Status = LeaveStatus.Pending,
                    ActionDate = DateTime.UtcNow
                };
                await context.AddAsync(request);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<LeaveRequestDTO>(request);
            }
            catch(Exception ex)
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
