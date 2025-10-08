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

            var record = await GetRecordOrThrow(recordId);               

            record.Status = newStatus;
            context.Update(record);
            await context.SaveChangesAsync();
            return mapper.Map<AttendanceRecordDTO>(record);
        }
        public async Task<List<AttendanceRecordDTO>> GetAllAttendanceRecords()
        {
            var records = await context.AttendanceRecords
                .Include(r => r.Employee)
                .OrderBy(r => r.ClockIn)
                .ToListAsync();

            var tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            foreach (var record in records)
            {
                record.ClockIn = TimeZoneInfo.ConvertTimeFromUtc(record.ClockIn, tz);
                if (record.ClockOut.HasValue)
                {
                    record.ClockOut = TimeZoneInfo.ConvertTimeFromUtc((DateTime)record.ClockOut, tz);
                }
            }

            return mapper.Map<List<AttendanceRecordDTO>>(records);
        }

        public async Task<AttendanceRecordDTO> GetTodayAttendanceForEmployeeAsync(string empId)
        {
            var emp = await GetUserOrThrow(empId);

            var attendanceRecord = await context.AttendanceRecords
                .Where(a => a.ClockIn >= DateTime.UtcNow.Date && a.EmployeeId == empId)
                .FirstOrDefaultAsync();

            if (attendanceRecord is not null)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                attendanceRecord.ClockIn = TimeZoneInfo.ConvertTimeFromUtc(attendanceRecord.ClockIn, tz);
                if (attendanceRecord.ClockOut.HasValue)
                {
                    attendanceRecord.ClockOut = TimeZoneInfo.ConvertTimeFromUtc((DateTime)attendanceRecord.ClockOut, tz);
                }
            }

            return mapper.Map<AttendanceRecordDTO>(attendanceRecord);
        }

        public async Task MarkAbsenteesAsync()
        {
            var today = DateTime.UtcNow.Date;

            if (today.DayOfWeek == DayOfWeek.Friday) return;

            var employees = await context.Users.ToListAsync();

            foreach (var emp in employees)
            {
                bool hasRecord = await context.AttendanceRecords
                    .AnyAsync(a => a.EmployeeId == emp.Id && a.ClockIn.Date == today);

                if (hasRecord) continue;

                bool hasApprovedLeave = await context.LeaveRequests
                    .AnyAsync(l => l.EmployeeId == emp.Id && 
                    l.Status == LeaveStatus.Approved &&
                    l.FromDate.Date <= today &&
                    l.ToDate.Date >= today);

                var record = new AttendanceRecord
                {
                    EmployeeId = emp.Id,
                    ClockIn = today,
                    Status = hasApprovedLeave ? AttendanceStatus.Leave : AttendanceStatus.Absent,
                    DeviceId = "System",
                    IpAddress = "System"
                };
                await context.AttendanceRecords.AddAsync(record);
            }
            await context.SaveChangesAsync();
        }

        public async Task<AttendanceRecordDTO> ClockInAsync(CreateAttendanceRecordDTO recordDTO)
        {
            var user = await GetUserOrThrow(recordDTO.EmployeeId);

            var existingRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == recordDTO.EmployeeId 
                && r.ClockIn.Date == DateTime.UtcNow.Date);

            if (existingRecord != null)
                throw new InvalidObjectException("تم تسجيل الحضور اليوم بالفعل.");

            var otherEmpsWithSameIp = await context.AttendanceRecords
                .Where(r => r.IpAddress == recordDTO.IpAddress && 
                r.DeviceId == recordDTO.DeviceId &&
                r.EmployeeId != recordDTO.EmployeeId && 
                r.ClockIn.Date == DateTime.UtcNow.Date)
                .Include(r => r.Employee)
                .ToListAsync();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var attendanceRecord = new AttendanceRecord
                {
                    EmployeeId = recordDTO.EmployeeId,
                    ClockIn = DateTime.UtcNow,
                    DeviceId = recordDTO.DeviceId,
                    IpAddress = recordDTO.IpAddress,
                    Status = AttendanceStatus.Present
                };

                logger.LogInformation("Creating new attendance record for employee {EmployeeId} at {CheckInTime}", recordDTO.EmployeeId, attendanceRecord.ClockIn);
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
                        { "CheckInTime", attendanceRecord.ClockIn.ToString("u") },
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
                throw new Exception(ex.Message);
            }
        }
        public async Task<AttendanceRecordDTO> ClockOutAsync(string empId)
        {
            var user = await GetUserOrThrow(empId);

            var existingRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == empId && r.ClockIn.Date == DateTime.UtcNow.Date);

            if (existingRecord == null)
                throw new InvalidObjectException("لم يتم تسجيل الحضور بعد اليوم.");

            if (existingRecord.ClockOut != null)
                throw new InvalidObjectException("تم تسجيل الانصراف بافعل");

            var otherEmpsWithSameIp = await context.AttendanceRecords
                .Where(r => r.IpAddress == existingRecord.IpAddress && 
                r.DeviceId == existingRecord.DeviceId &&
                r.EmployeeId != existingRecord.EmployeeId && 
                r.ClockIn.Date == DateTime.UtcNow.Date)
                .Include(r => r.Employee)
                .ToListAsync();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                existingRecord.ClockOut = DateTime.UtcNow;


                //logger.LogInformation("Creating new attendance record for employee {EmployeeId} at {CheckInTime}", recordDTO.EmployeeId, attendanceRecord.ClockIn);
                context.Update(existingRecord);
                await context.SaveChangesAsync();

                //logger.LogInformation("Attendance record created with Id {RecordId}", attendanceRecord.Id);

                if (otherEmpsWithSameIp.Any())
                {
                    var otherEmployeesNames = string.Join(", ",
                        otherEmpsWithSameIp.Select(r => $"{r.Employee.FirstName} {r.Employee.LastName}"));

                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        { "CurrentEmpName", $"{user.FirstName} {user.LastName}" },
                        { "CurrentEmpEmail", user.Email ?? string.Empty },
                        { "CurrentEmpId", user.Id },
                        { "SuspiciousIp", existingRecord.IpAddress },
                        { "CheckInTime", existingRecord.ClockIn.ToString("u") },
                        { "DeviceId", existingRecord.DeviceId },
                        { "OtherEmployees", otherEmployeesNames },
                        { "TotalEmployeesOnIp", (otherEmpsWithSameIp.Count + 1).ToString() },
                    };

                    await emailService.SendEmailWithTemplateAsync("zminaretagency@gmail.com", "Attendance Alert : Multiple Employees Using Same Device", "AttendanceAlert", replacements);
                }

                await transaction.CommitAsync();
                return mapper.Map<AttendanceRecordDTO>(existingRecord);
            }
            catch (Exception ex)
            {
                logger.LogError("Error occurred while creating attendance record for employee {EmployeeId}, with error message: {ex}", existingRecord.EmployeeId, ex.Message);
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        private async Task<ApplicationUser> GetUserOrThrow(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new InvalidObjectException($"لم يتم العثور على الموظف بهذا المعرف {userId}");
            return user;
        }
        private async Task<AttendanceRecord> GetRecordOrThrow(int recordId)
        {
            var record = await context.AttendanceRecords
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == recordId)
                ?? throw new InvalidObjectException($"لم يتم العثور على الحضور بهذا المعرف {recordId}");
            return record;
        }

    }
}
