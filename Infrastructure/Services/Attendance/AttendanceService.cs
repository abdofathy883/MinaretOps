using AutoMapper;
using Core.DTOs.Attendance;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services.Attendance
{
    public class AttendanceService : IAttendanceService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public AttendanceService(
            MinaretOpsDbContext dbContext,
            IMapper _mapper,
            UserManager<ApplicationUser> _userManager
            )
        {
            context = dbContext;
            mapper = _mapper;
            userManager = _userManager;
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
        public async Task<List<AttendanceRecordDTO>> GetAllAttendanceRecords(DateOnly date)
        {
            var records = await context.AttendanceRecords
                .Include(r => r.Employee)
                .Where(r => r.ClockIn.Date == date.ToDateTime(TimeOnly.MinValue).Date)
                .OrderBy(r => r.ClockIn)
                .ToListAsync();

            return mapper.Map<List<AttendanceRecordDTO>>(records);
        }

        public async Task<PaginatedAttendanceResultDTO> GetAttendanceRecordsAsync(AttendanceFilterDTO filter)
        {
            var query = context.AttendanceRecords
                .Include(r => r.Employee)
                .Include(r => r.BreakPeriods)
                .AsQueryable();

            // Apply date range filter
            if (filter.FromDate.HasValue)
            {
                query = query.Where(r => r.WorkDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(r => r.WorkDate <= filter.ToDate.Value);
            }

            // Apply employee filter
            if (!string.IsNullOrEmpty(filter.EmployeeId))
            {
                query = query.Where(r => r.EmployeeId == filter.EmployeeId);
            }

            // Get total count before pagination
            var totalRecords = await query.CountAsync();

            // Apply ordering and pagination
            var records = await query
                .OrderByDescending(r => r.WorkDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize);

            return new PaginatedAttendanceResultDTO
            {
                Records = mapper.Map<List<AttendanceRecordDTO>>(records),
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<AttendanceRecordDTO> GetTodayAttendanceForEmployeeAsync(string empId)
        {
            var emp = await GetUserOrThrow(empId);

            var egyptToday = TimeZoneHelper.GetEgyptToday();

            var attendanceRecord = await context.AttendanceRecords
                .Include(r => r.BreakPeriods)
                .FirstOrDefaultAsync(a => a.EmployeeId == emp.Id && a.WorkDate == egyptToday);

            return mapper.Map<AttendanceRecordDTO>(attendanceRecord);
        }

        //public async Task MarkAbsenteesAsync()
        //{
        //    var egyptYesterday = TimeZoneHelper.GetEgyptYesterday();

        //    var yesterdayDayOfWeek = egyptYesterday.ToDateTime(TimeOnly.MinValue).DayOfWeek;
        //    if (yesterdayDayOfWeek == DayOfWeek.Friday) return;

        //    var employees = await context.Users.ToListAsync();

        //    foreach (var emp in employees)
        //    {
        //        var existingRecord = await context.AttendanceRecords
        //            .FirstOrDefaultAsync(a => a.EmployeeId == emp.Id && a.WorkDate == egyptYesterday);

        //        var yesterdayDateTime = egyptYesterday.ToDateTime(TimeOnly.MinValue);
        //        bool hasApprovedLeave = await context.LeaveRequests
        //            .AnyAsync(l => l.EmployeeId == emp.Id &&
        //            l.Status == LeaveStatus.Approved &&
        //            l.FromDate.Date <= yesterdayDateTime &&
        //            l.ToDate.Date >= yesterdayDateTime);

        //        // Case 1: Employee has no attendance record at all
        //        if (existingRecord == null)
        //        {
        //            var record = new AttendanceRecord
        //            {
        //                EmployeeId = emp.Id,
        //                WorkDate = egyptYesterday,
        //                ClockIn = yesterdayDateTime,
        //                ClockOut = yesterdayDateTime.AddHours(23).AddMinutes(59),
        //                Status = hasApprovedLeave ? AttendanceStatus.Leave : AttendanceStatus.Absent,
        //                DeviceId = "System",
        //                IpAddress = "System"
        //            };
        //            await context.AttendanceRecords.AddAsync(record);
        //        }
        //        // Case 2: Employee clocked in but didn't clock out (missing clock out)
        //        else if (existingRecord.ClockOut == null && existingRecord.Status == AttendanceStatus.Present)
        //        {
        //            // Close the record at end of yesterday
        //            existingRecord.ClockOut = yesterdayDateTime.AddHours(23).AddMinutes(59);
        //            existingRecord.MissingClockOut = true;
        //            context.Update(existingRecord);
        //        }
        //    }
        //    await context.SaveChangesAsync();
        //}

        public async Task MarkAbsenteesAsync()
        {
            var egyptYesterday = TimeZoneHelper.GetEgyptYesterday();
            var egyptToday = TimeZoneHelper.GetEgyptToday();
            var egyptTomorrow = egyptToday.AddDays(1);

            // Process yesterday's absentees
            var yesterdayDayOfWeek = egyptYesterday.ToDateTime(TimeOnly.MinValue).DayOfWeek;
            if (yesterdayDayOfWeek != DayOfWeek.Friday)
            {
                var employees = await context.Users.ToListAsync();

                foreach (var emp in employees)
                {
                    var existingRecord = await context.AttendanceRecords
                        .FirstOrDefaultAsync(a => a.EmployeeId == emp.Id && a.WorkDate == egyptYesterday);

                    var yesterdayDateTime = egyptYesterday.ToDateTime(TimeOnly.MinValue);
                    bool hasApprovedLeave = await context.LeaveRequests
                        .AnyAsync(l => l.EmployeeId == emp.Id &&
                        l.Status == LeaveStatus.Approved &&
                        l.FromDate.Date <= yesterdayDateTime &&
                        l.ToDate.Date >= yesterdayDateTime);

                    // Case 1: Employee has no attendance record at all
                    if (existingRecord == null)
                    {
                        var record = new AttendanceRecord
                        {
                            EmployeeId = emp.Id,
                            WorkDate = egyptYesterday,
                            ClockIn = yesterdayDateTime,
                            ClockOut = yesterdayDateTime,
                            Status = hasApprovedLeave ? AttendanceStatus.Leave : AttendanceStatus.Absent,
                            DeviceId = "System",
                            IpAddress = "System"
                        };
                        await context.AttendanceRecords.AddAsync(record);
                    }
                    // Case 2: Employee clocked in but didn't clock out (missing clock out)
                    else if (existingRecord.ClockOut == null && existingRecord.Status == AttendanceStatus.Present)
                    {
                        // Close the record at end of yesterday
                        existingRecord.ClockOut = yesterdayDateTime;
                        existingRecord.MissingClockOut = true;
                        context.Update(existingRecord);
                    }
                }
            }

            // Process tomorrow's leaves (create records for employees with approved leave for tomorrow)
            var tomorrowDayOfWeek = egyptTomorrow.ToDateTime(TimeOnly.MinValue).DayOfWeek;
            if (tomorrowDayOfWeek != DayOfWeek.Friday)
            {
                var tomorrowDateTime = egyptTomorrow.ToDateTime(TimeOnly.MinValue);

                // Find all employees with approved leave requests for tomorrow
                var employeesWithLeaveForTomorrow = await context.LeaveRequests
                    .Where(l => l.Status == LeaveStatus.Approved &&
                            l.FromDate.Date <= tomorrowDateTime &&
                            l.ToDate.Date >= tomorrowDateTime)
                    .Select(l => l.EmployeeId)
                    .Distinct()
                    .ToListAsync();

                // Check if we already have records for tomorrow (shouldn't happen, but just to be safe)
                var existingTomorrowRecords = await context.AttendanceRecords
                    .Where(a => a.WorkDate == egyptTomorrow)
                    .Select(a => a.EmployeeId)
                    .ToListAsync();

                // Create records for employees with leave for tomorrow who don't already have a record
                foreach (var empId in employeesWithLeaveForTomorrow)
                {
                    if (!existingTomorrowRecords.Contains(empId))
                    {
                        var record = new AttendanceRecord
                        {
                            EmployeeId = empId,
                            WorkDate = egyptTomorrow,
                            ClockIn = tomorrowDateTime,
                            ClockOut = tomorrowDateTime,
                            Status = AttendanceStatus.Leave,
                            DeviceId = "System",
                            IpAddress = "System"
                        };
                        await context.AttendanceRecords.AddAsync(record);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
        public async Task<AttendanceRecordDTO> ClockInAsync(CreateAttendanceRecordDTO recordDTO)
        {
            var user = await GetUserOrThrow(recordDTO.EmployeeId);

            var egyptToday = TimeZoneHelper.GetEgyptToday();

            var existingRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == recordDTO.EmployeeId 
                && r.WorkDate == egyptToday);

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
                    WorkDate = egyptToday,
                    DeviceId = recordDTO.DeviceId,
                    IpAddress = recordDTO.IpAddress,
                    Status = AttendanceStatus.Present
                };
                await context.AddAsync(attendanceRecord);

                if (otherEmpsWithSameIp.Any())
                {
                    var otherEmployeesNames = string.Join(", ",
                        otherEmpsWithSameIp.Select(r => $"{r.Employee.FirstName} {r.Employee.LastName}"));

                    var emailPayload = new
                    {
                        To = "zminaretagency@gmail.com",
                        Subject = "Attendance Alert : Multiple Employees Using Same Device",
                        Template = "AttendanceAlert",
                        Replacements = new Dictionary<string, string>
                        {
                            { "CurrentEmpName", $"{user.FirstName} {user.LastName}" },
                            { "CurrentEmpEmail", user.Email ?? string.Empty },
                            { "CurrentEmpId", user.Id },
                            { "SuspiciousIp", recordDTO.IpAddress },
                            { "CheckInTime", attendanceRecord.ClockIn.ToString("u") },
                            { "DeviceId", recordDTO.DeviceId },
                            { "OtherEmployees", otherEmployeesNames },
                            { "TotalEmployeesOnIp", (otherEmpsWithSameIp.Count + 1).ToString() }
                        }
                    };

                    var emailOutbox = new Outbox
                    {
                        OpTitle = "Attendance Alert Email",
                        OpType = OutboxTypes.Email,
                        PayLoad = JsonSerializer.Serialize(emailPayload)
                    };
                    await context.OutboxMessages.AddAsync(emailOutbox);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<AttendanceRecordDTO>(attendanceRecord);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<AttendanceRecordDTO> ClockOutAsync(string empId)
        {
            var user = await GetUserOrThrow(empId);

            var egyptToday = TimeZoneHelper.GetEgyptToday();

            var existingRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == empId && r.WorkDate == egyptToday);

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
                existingRecord.MissingClockOut = false;

                context.Update(existingRecord);

                if (otherEmpsWithSameIp.Any())
                {
                    var otherEmployeesNames = string.Join(", ",
                        otherEmpsWithSameIp.Select(r => $"{r.Employee.FirstName} {r.Employee.LastName}"));

                    var emailPayload = new
                    {
                        To = "zminaretagency@gmail.com",
                        Subject = "Attendance Alert : Multiple Employees Using Same Device",
                        Template = "AttendanceAlert",
                        Replacements = new Dictionary<string, string>
                        {
                            { "CurrentEmpName", $"{user.FirstName} {user.LastName}" },
                            { "CurrentEmpEmail", user.Email ?? string.Empty },
                            { "CurrentEmpId", user.Id },
                            { "SuspiciousIp", existingRecord.IpAddress },
                            { "CheckInTime", existingRecord.ClockIn.ToString("u") },
                            { "DeviceId", existingRecord.DeviceId },
                            { "OtherEmployees", otherEmployeesNames },
                            { "TotalEmployeesOnIp", (otherEmpsWithSameIp.Count + 1).ToString() }
                        }
                    };

                    var emailOutbox = new Outbox
                    {
                        OpTitle = "Attendance Alert Email",
                        OpType = OutboxTypes.Email,
                        PayLoad = JsonSerializer.Serialize(emailPayload)
                    };

                    await context.OutboxMessages.AddAsync(emailOutbox);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<AttendanceRecordDTO>(existingRecord);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
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

        public async Task<bool> SubmitEarlyLeaveByEmpIdAsync(ToggleEarlyLeaveDTO earlyLeave)
        {
            var emp = await GetUserOrThrow(earlyLeave.EmployeeId);

            var attendanceRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.EmployeeId == emp.Id && a.WorkDate == earlyLeave.WorkDate)
                ?? throw new Exception("لم يتم العثور على سجل الحضور للموظف.");

            attendanceRecord.EarlyLeave = true;
            context.Update(attendanceRecord);
            return await context.SaveChangesAsync() > 0;
        }

        //public Task<List<AttendanceRecordDTO>> GetMonthlyReportForEmpAsync()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
