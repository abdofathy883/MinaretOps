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

namespace Infrastructure.Services.Attendance
{
    public class AttendanceService : IAttendanceService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AttendanceService> logger;
        public AttendanceService(
            MinaretOpsDbContext dbContext,
            IEmailService _emailService,
            IMapper _mapper,
            UserManager<ApplicationUser> _userManager,
            ILogger<AttendanceService> _logger
            )
        {
            context = dbContext;
            emailService = _emailService;
            mapper = _mapper;
            userManager = _userManager;
            logger = _logger;
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

        public async Task<List<AttendanceRecordDTO>> GetAllAttendanceRecords()
        {
            var records = await context.AttendanceRecords
                .Include(r => r.Employee)
                .ToListAsync();

            var tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            foreach (var record in records)
            {
                record.CheckInTime = TimeZoneInfo.ConvertTimeFromUtc(record.CheckInTime, tz);
            }

            return mapper.Map<List<AttendanceRecordDTO>>(records);
        }

        public async Task<List<AttendanceRecordDTO>> GetAttendanceRecordsByEmployee(string employeeId)
        {
            var emp = await GetUserOrThrow(employeeId);

            var records = await context.AttendanceRecords
                .Where(r => r.EmployeeId == employeeId)
                .ToListAsync();

            logger.LogInformation("Fetched {Count} attendance records for employee {EmployeeId}", records.Count, employeeId);

            return mapper.Map<List<AttendanceRecordDTO>>(records);
        }

        public async Task<AttendanceRecordDTO> GetTodayAttendanceForEmployeeAsync(string empId)
        {
            var emp = await GetUserOrThrow(empId);

            var attendanceRecord = await context.AttendanceRecords
                .Where(a => a.CheckInTime >= DateTime.UtcNow.Date && a.EmployeeId == empId)
                .FirstOrDefaultAsync();

            if (attendanceRecord is not null)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                attendanceRecord.CheckInTime = TimeZoneInfo.ConvertTimeFromUtc(attendanceRecord.CheckInTime, tz);
            }

            return mapper.Map<AttendanceRecordDTO>(attendanceRecord);
        }

        public async Task MarkAbsenteesAsync()
        {
            var now = DateTime.UtcNow.Date; // only work with whole days
            var yesterday = now.AddDays(-1);

            // skip Friday
            if (now.DayOfWeek == DayOfWeek.Friday) return;

            // get all employees
            var employees = await context.Users.ToListAsync();

            foreach (var emp in employees)
            {
                // find latest attendance record
                var lastRecord = await context.AttendanceRecords
                    .Where(a => a.EmployeeId == emp.Id)
                    .OrderByDescending(a => a.CheckInTime)
                    .FirstOrDefaultAsync();

                if (lastRecord == null) continue;
                if (lastRecord.CheckInTime.Date >= yesterday) continue;

                var lastDate = lastRecord.CheckInTime.Date;
                // Fill missing days up to yesterday
                for (var missingDate = lastDate.AddDays(1); missingDate <= yesterday; missingDate = missingDate.AddDays(1))
                {
                    if (missingDate.DayOfWeek == DayOfWeek.Friday)
                        continue;

                    var record = new AttendanceRecord
                    {
                        EmployeeId = emp.Id,
                        CheckInTime = missingDate, // Date only
                        Status = AttendanceStatus.Absent,
                        DeviceId = "System",
                        IpAddress = "System"
                    };
                    context.AttendanceRecords.Add(record);
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<AttendanceRecordDTO> NewAttendanceRecord(CreateAttendanceRecordDTO recordDTO)
        {
            if (recordDTO is null)
            {
                logger.LogError("Attendance Record Coming From Frontend is Null");
                throw new InvalidObjectException("Attendance Record Coming From Frontend is Empty");
            }

            var user = await GetUserOrThrow(recordDTO.EmployeeId);

            var existingRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == recordDTO.EmployeeId && r.CheckInTime.Date == DateTime.UtcNow.Date);

            if (existingRecord != null)
                throw new InvalidObjectException("Attendance record for today already exists.");

            var otherEmpsWithSameIp = await context.AttendanceRecords
                .Where(r => r.IpAddress == recordDTO.IpAddress && 
                r.DeviceId == recordDTO.DeviceId &&
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
                    Status = AttendanceStatus.Present
                };

                logger.LogInformation("Creating new attendance record for employee {EmployeeId} at {CheckInTime}", recordDTO.EmployeeId, attendanceRecord.CheckInTime);
                await context.AddAsync(attendanceRecord);
                await context.SaveChangesAsync();

                logger.LogInformation("Attendance record created with Id {RecordId}", attendanceRecord.Id);

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
            catch (Exception ex)
            {
                logger.LogError("Error occurred while creating attendance record for employee {EmployeeId}, with error message: {ex}", recordDTO.EmployeeId, ex.Message);
                await transaction.RollbackAsync();
                throw;
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
