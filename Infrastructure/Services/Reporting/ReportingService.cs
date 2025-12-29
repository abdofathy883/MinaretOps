using AutoMapper;
using Core.DTOs.Reporting;
using Core.DTOs.Tasks.TaskDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Reporting
{
    public class ReportingService : IReportingService
    {
        private readonly MinaretOpsDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public ReportingService(MinaretOpsDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<TaskEmployeeReportDTO> GetTaskEmployeeReportAsync(string currentUserId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var egyptToday = TimeZoneHelper.GetEgyptToday();
            var egyptNow = TimeZoneHelper.GetEgyptNow();
            var utcNow = DateTime.UtcNow;
            var todayDateTime = egyptToday.ToDateTime(TimeOnly.MinValue);

            // Determine date range - default to today if not provided
            DateOnly fromDateOnly;
            DateOnly toDateOnly;
            
            if (fromDate.HasValue && toDate.HasValue)
            {
                fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                toDateOnly = DateOnly.FromDateTime(toDate.Value);
            }
            else if (fromDate.HasValue)
            {
                fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                toDateOnly = fromDateOnly;
            }
            else if (toDate.HasValue)
            {
                toDateOnly = DateOnly.FromDateTime(toDate.Value);
                fromDateOnly = toDateOnly;
            }
            else
            {
                // Default to today if no dates provided
                fromDateOnly = egyptToday;
                toDateOnly = egyptToday;
            }

            // Check if today is within the date range (for determining if we should show "On Break" employees)
            bool includeToday = egyptToday >= fromDateOnly && egyptToday <= toDateOnly;

            // Get attendance data for the date range
            // For today: get active records (ClockOut == null) to check for breaks
            // For historical dates: get all records regardless of ClockOut status
            var attendanceRecordsQuery = context.AttendanceRecords
                .Include(r => r.Employee)
                .Include(r => r.BreakPeriods)
                .Where(r => r.WorkDate >= fromDateOnly && r.WorkDate <= toDateOnly &&
                           r.Status == AttendanceStatus.Present);

            // If today is included, we need active records to check for breaks
            // Otherwise, get all records for the date range
            if (includeToday)
            {
                // For today, get active records (not clocked out) to check break status
                var activeAttendanceRecords = await attendanceRecordsQuery
                    .Where(r => r.WorkDate == egyptToday && r.ClockOut == null)
                    .ToListAsync();

                // Also get completed records for today (already clocked out)
                var completedTodayRecords = await attendanceRecordsQuery
                    .Where(r => r.WorkDate == egyptToday && r.ClockOut != null)
                    .ToListAsync();

                // Get historical records (other dates in range)
                var historicalRecords = await attendanceRecordsQuery
                    .Where(r => r.WorkDate != egyptToday)
                    .ToListAsync();

                // Separate working and on-break employees (only for today's active records)
                var workingEmployees = new List<EmployeeWithTasksDTO>();
                var onBreakEmployees = new List<EmployeeWithTasksDTO>();

                // Process active records for today (can be on break)
                foreach (var record in activeAttendanceRecords)
                {
                    var isOnBreak = record.BreakPeriods.Any(b => b.EndTime == null);
                        var employeeInfo = new EmployeeWithTasksDTO
                    {
                        EmployeeId = record.EmployeeId,
                        EmployeeName = $"{record.Employee.FirstName} {record.Employee.LastName}",
                        IsOnBreak = isOnBreak,
                            ClockInTime = TimeZoneHelper.ConvertToEgyptTime(record.ClockIn),
                            WorkingDuration = isOnBreak ? null : utcNow - record.ClockIn
                    };

                    if (isOnBreak)
                    {
                        onBreakEmployees.Add(employeeInfo);
                    }
                    else
                    {
                        workingEmployees.Add(employeeInfo);
                    }
                }

                // Process completed records for today (already clocked out - add to working)
                foreach (var record in completedTodayRecords)
                {
                    // Check if employee is already in workingEmployees (from active records)
                    if (!workingEmployees.Any(e => e.EmployeeId == record.EmployeeId) &&
                        !onBreakEmployees.Any(e => e.EmployeeId == record.EmployeeId))
                    {
                        var clockOutTime = record.ClockOut ?? utcNow;
                        var workingDuration = clockOutTime - record.ClockIn;
                        workingEmployees.Add(new EmployeeWithTasksDTO
                        {
                            EmployeeId = record.EmployeeId,
                            EmployeeName = $"{record.Employee.FirstName} {record.Employee.LastName}",
                            IsOnBreak = false,
                            ClockInTime = TimeZoneHelper.ConvertToEgyptTime(record.ClockIn),
                            WorkingDuration = workingDuration
                        });
                    }
                }

                // Process historical records (all go to workingEmployees)
                foreach (var record in historicalRecords)
                {
                    // Check if employee is already in the lists
                    if (!workingEmployees.Any(e => e.EmployeeId == record.EmployeeId) &&
                        !onBreakEmployees.Any(e => e.EmployeeId == record.EmployeeId))
                    {
                        var clockOutTime = record.ClockOut ?? utcNow;
                        var workingDuration = clockOutTime - record.ClockIn;
                        workingEmployees.Add(new EmployeeWithTasksDTO
                        {
                            EmployeeId = record.EmployeeId,
                            EmployeeName = $"{record.Employee.FirstName} {record.Employee.LastName}",
                            IsOnBreak = false,
                            ClockInTime = TimeZoneHelper.ConvertToEgyptTime(record.ClockIn),
                            WorkingDuration = workingDuration
                        });
                    }
                }

                // Get all employees for absent check
                var allEmployees = await context.Users
                    .Where(u => !u.IsDeleted)
                    .ToListAsync();

                var employeeIdsWithAttendance = activeAttendanceRecords
                    .Select(r => r.EmployeeId)
                    .Concat(completedTodayRecords.Select(r => r.EmployeeId))
                    .Concat(historicalRecords.Select(r => r.EmployeeId))
                    .Distinct()
                    .ToList();

                // Get absent employees (check for each date in range)
                var absentEmployees = new List<EmployeeWithTasksDTO>();
                var dateRange = new List<DateOnly>();
                for (var date = fromDateOnly; date <= toDateOnly; date = date.AddDays(1))
                {
                    dateRange.Add(date);
                }

                foreach (var employee in allEmployees)
                {
                    foreach (var date in dateRange)
                    {
                        // Skip if employee already has attendance for this date
                        var hasAttendanceForDate = await context.AttendanceRecords
                            .AnyAsync(r => r.EmployeeId == employee.Id && 
                                         r.WorkDate == date && 
                                         r.Status == AttendanceStatus.Present);

                        if (hasAttendanceForDate)
                            continue;

                        // Check if employee has approved leave for this date
                        var dateDateTime = date.ToDateTime(TimeOnly.MinValue);
                        var hasApprovedLeave = await context.LeaveRequests
                            .AnyAsync(l => l.EmployeeId == employee.Id &&
                                         l.Status == LeaveStatus.Approved &&
                                         l.FromDate.Date <= dateDateTime &&
                                         l.ToDate.Date >= dateDateTime);

                        if (!hasApprovedLeave)
                        {
                            // Check if it's a working day (not Friday)
                            var isWorkingDay = dateDateTime.DayOfWeek != DayOfWeek.Friday;
                            if (isWorkingDay)
                            {
                                // Only add if not already in absent list
                                if (!absentEmployees.Any(e => e.EmployeeId == employee.Id))
                                {
                                    absentEmployees.Add(new EmployeeWithTasksDTO
                                    {
                                        EmployeeId = employee.Id,
                                        EmployeeName = $"{employee.FirstName} {employee.LastName}"
                                    });
                                }
                            }
                        }
                    }
                }

                // Get all employee IDs for task query
                var allEmployeeIds = workingEmployees.Select(e => e.EmployeeId)
                    .Concat(onBreakEmployees.Select(e => e.EmployeeId))
                    .Concat(absentEmployees.Select(e => e.EmployeeId))
                    .ToList();

                // Initialize task dictionary
                var taskDictionary = new Dictionary<string, List<LightWieghtTaskDTO>>();

                if (allEmployeeIds.Any())
                {
                    IQueryable<TaskItem> query = context.Tasks
                        .Include(t => t.ClientService)
                            .ThenInclude(cs => cs.Service)
                        .Include(t => t.ClientService)
                            .ThenInclude(cs => cs.Client)
                        .Include(t => t.Employee)
                        .Where(t => allEmployeeIds.Contains(t.EmployeeId) 
                        && t.Status != CustomTaskStatus.Completed
                        && t.Status != CustomTaskStatus.Rejected);

                    // Load all tasks in a single query
                    var allTasks = await query.ToListAsync();

                    // Group tasks by EmployeeId before mapping to DTO
                    var tasksGroupedByEmployee = allTasks
                        .GroupBy(t => t.EmployeeId)
                        .ToList();

                    // Map each group to DTOs and add to dictionary
                    foreach (var group in tasksGroupedByEmployee)
                    {
                        var tasksDTO = mapper.Map<List<LightWieghtTaskDTO>>(group.ToList());
                        taskDictionary[group.Key ?? string.Empty] = tasksDTO;
                    }
                }

                // Assign tasks to employees
                foreach (var emp in workingEmployees)
                {
                    emp.Tasks = taskDictionary.GetValueOrDefault(emp.EmployeeId, new List<LightWieghtTaskDTO>());
                }

                foreach (var emp in onBreakEmployees)
                {
                    emp.Tasks = taskDictionary.GetValueOrDefault(emp.EmployeeId, new List<LightWieghtTaskDTO>());
                }

                foreach (var emp in absentEmployees)
                {
                    emp.Tasks = taskDictionary.GetValueOrDefault(emp.EmployeeId, new List<LightWieghtTaskDTO>());
                }

                return new TaskEmployeeReportDTO
                {
                    WorkingEmployees = workingEmployees.OrderBy(e => e.EmployeeName).ToList(),
                    OnBreakEmployees = onBreakEmployees.OrderBy(e => e.EmployeeName).ToList(),
                    AbsentEmployees = absentEmployees.OrderBy(e => e.EmployeeName).ToList()
                };
            }
            else
            {
                // Historical dates only - no "On Break" employees
                var attendanceRecords = await attendanceRecordsQuery.ToListAsync();

                var workingEmployees = new List<EmployeeWithTasksDTO>();

                // Process all attendance records (all go to workingEmployees)
                foreach (var record in attendanceRecords)
                {
                    var clockOutTime = record.ClockOut ?? utcNow;
                    var workingDuration = clockOutTime - record.ClockIn;
                    workingEmployees.Add(new EmployeeWithTasksDTO
                    {
                        EmployeeId = record.EmployeeId,
                        EmployeeName = $"{record.Employee.FirstName} {record.Employee.LastName}",
                        IsOnBreak = false,
                        ClockInTime = TimeZoneHelper.ConvertToEgyptTime(record.ClockIn),
                        WorkingDuration = workingDuration
                    });
                }

                // Get all employees for absent check
                var allEmployees = await context.Users
                    .Where(u => !u.IsDeleted)
                    .ToListAsync();

                var employeeIdsWithAttendance = attendanceRecords
                    .Select(r => r.EmployeeId)
                    .Distinct()
                    .ToList();

                // Get absent employees (check for each date in range)
                var absentEmployees = new List<EmployeeWithTasksDTO>();
                var dateRange = new List<DateOnly>();
                for (var date = fromDateOnly; date <= toDateOnly; date = date.AddDays(1))
                {
                    dateRange.Add(date);
                }

                foreach (var employee in allEmployees)
                {
                    foreach (var date in dateRange)
                    {
                        // Skip if employee already has attendance for this date
                        var hasAttendanceForDate = await context.AttendanceRecords
                            .AnyAsync(r => r.EmployeeId == employee.Id && 
                                         r.WorkDate == date && 
                                         r.Status == AttendanceStatus.Present);

                        if (hasAttendanceForDate)
                            continue;

                        // Check if employee has approved leave for this date
                        var dateDateTime = date.ToDateTime(TimeOnly.MinValue);
                        var hasApprovedLeave = await context.LeaveRequests
                            .AnyAsync(l => l.EmployeeId == employee.Id &&
                                         l.Status == LeaveStatus.Approved &&
                                         l.FromDate.Date <= dateDateTime &&
                                         l.ToDate.Date >= dateDateTime);

                        if (!hasApprovedLeave)
                        {
                            // Check if it's a working day (not Friday)
                            var isWorkingDay = dateDateTime.DayOfWeek != DayOfWeek.Friday;
                            if (isWorkingDay)
                            {
                                // Only add if not already in absent list
                                if (!absentEmployees.Any(e => e.EmployeeId == employee.Id))
                                {
                                    absentEmployees.Add(new EmployeeWithTasksDTO
                                    {
                                        EmployeeId = employee.Id,
                                        EmployeeName = $"{employee.FirstName} {employee.LastName}"
                                    });
                                }
                            }
                        }
                    }
                }

                // Get all employee IDs for task query
                var allEmployeeIds = workingEmployees.Select(e => e.EmployeeId)
                    .Concat(absentEmployees.Select(e => e.EmployeeId))
                    .ToList();

                // Initialize task dictionary
                var taskDictionary = new Dictionary<string, List<LightWieghtTaskDTO>>();

                if (allEmployeeIds.Any())
                {
                    IQueryable<TaskItem> query = context.Tasks
                        .Include(t => t.ClientService)
                            .ThenInclude(cs => cs.Service)
                        .Include(t => t.ClientService)
                            .ThenInclude(cs => cs.Client)
                        .Include(t => t.Employee)
                        .Where(t => allEmployeeIds.Contains(t.EmployeeId) 
                        && t.Status != CustomTaskStatus.Completed
                        && t.Status != CustomTaskStatus.Rejected);

                    // Load all tasks in a single query
                    var allTasks = await query.ToListAsync();

                    // Group tasks by EmployeeId before mapping to DTO
                    var tasksGroupedByEmployee = allTasks
                        .GroupBy(t => t.EmployeeId)
                        .ToList();

                    // Map each group to DTOs and add to dictionary
                    foreach (var group in tasksGroupedByEmployee)
                    {
                        var tasksDTO = mapper.Map<List<LightWieghtTaskDTO>>(group.ToList());
                        taskDictionary[group.Key ?? string.Empty] = tasksDTO;
                    }
                }

                // Assign tasks to employees
                foreach (var emp in workingEmployees)
                {
                    emp.Tasks = taskDictionary.GetValueOrDefault(emp.EmployeeId, new List<LightWieghtTaskDTO>());
                }

                foreach (var emp in absentEmployees)
                {
                    emp.Tasks = taskDictionary.GetValueOrDefault(emp.EmployeeId, new List<LightWieghtTaskDTO>());
                }

                return new TaskEmployeeReportDTO
                {
                    WorkingEmployees = workingEmployees.OrderBy(e => e.EmployeeName).ToList(),
                    OnBreakEmployees = new List<EmployeeWithTasksDTO>(), // Empty for historical dates
                    AbsentEmployees = absentEmployees.OrderBy(e => e.EmployeeName).ToList()
                };
            }
        }
        public async Task<MonthlyAttendanceReportDTO> GetMonthlyAttendanceReportAsync(DateTime fromDate, DateTime toDate, AttendanceStatus? status = null)
        {
            // Convert to DateOnly for comparison
            var fromDateOnly = DateOnly.FromDateTime(fromDate);
            var toDateOnly = DateOnly.FromDateTime(toDate);

            if (fromDateOnly > toDateOnly)
            {
                throw new ArgumentException("FromDate cannot be greater than ToDate");
            }

            var utcNow = DateTime.UtcNow;
            var fromEgyptStart = fromDateOnly.ToDateTime(TimeOnly.MinValue);
            var toEgyptEnd = toDateOnly.ToDateTime(TimeOnly.MaxValue);
            var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromEgyptStart, TimeZoneHelper.EgyptTimeZone);
            var toUtc = TimeZoneInfo.ConvertTimeToUtc(toEgyptEnd, TimeZoneHelper.EgyptTimeZone);

            // Get all attendance records for the date range
            var attendanceRecordsQuery = context.AttendanceRecords
                .Include(r => r.Employee)
                .Include(r => r.BreakPeriods)
                .Where(r => r.WorkDate >= fromDateOnly && r.WorkDate <= toDateOnly);

            var incidentsquery = await context.KPIIncedints
                .Include(i => i.Employee)
                .Where(i => i.TimeStamp >= fromUtc && i.TimeStamp <= toUtc)
                .ToListAsync();

            // Apply status filter if provided
            if (status.HasValue)
            {
                attendanceRecordsQuery = attendanceRecordsQuery.Where(r => r.Status == status.Value);
            }

            var attendanceRecords = await attendanceRecordsQuery.ToListAsync();

            // Get all approved leave requests that overlap with the date range
            var leaveRequests = await context.LeaveRequests
                .Where(l => l.Status == LeaveStatus.Approved &&
                           l.FromDate.Date <= toDate &&
                           l.ToDate.Date >= fromDate)
                .ToListAsync();

            // When status filter is applied, only get employees who have records matching that status
            // Otherwise, get all employees
            List<ApplicationUser> employeesToProcess;
            if (status.HasValue)
            {
                // Get distinct employee IDs from filtered records
                var employeeIdsWithMatchingStatus = attendanceRecords
                    .Select(r => r.EmployeeId)
                    .Distinct()
                    .ToList();

                employeesToProcess = await context.Users
                    .Where(u => !u.IsDeleted && employeeIdsWithMatchingStatus.Contains(u.Id))
                    .ToListAsync();
            }
            else
            {
                // Get all employees when no status filter
                employeesToProcess = await context.Users
                    .Where(u => !u.IsDeleted)
                    .ToListAsync();
            }

            var report = new MonthlyAttendanceReportDTO
            {
                FromDate = fromDate.Date,
                ToDate = toDate.Date,
                Employees = new List<EmployeeMonthlyAttendanceDTO>()
            };

            foreach (var employee in employeesToProcess)
            {
                var employeeRecords = attendanceRecords
                    .Where(r => r.EmployeeId == employee.Id)
                    .ToList();

                var employeeIncidents = incidentsquery
                   .Where(i => i.EmployeeId == employee.Id)
                   .Select(i => new EmployeeMonthlyIncidentsDTO
                   {
                       AspectType = i.Aspect,
                       CreatedAt = TimeZoneHelper.ConvertToEgyptTime(i.TimeStamp)
                   })
                   .ToList();

                var employeeAttendance = new EmployeeMonthlyAttendanceDTO
                {
                    EmployeeId = employee.Id,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    TotalDaysPresent = employeeRecords.Count(r => r.Status == AttendanceStatus.Present),
                    TotalDaysAbsent = employeeRecords.Count(r => r.Status == AttendanceStatus.Absent),
                    TotalDaysOnLeave = employeeRecords.Count(r => r.Status == AttendanceStatus.Leave),
                    TotalMissingClockout = employeeRecords.Count(r => r.MissingClockOut == true),
                    Incidents = employeeIncidents
                };

                // Calculate total hours worked
                double totalHours = 0;
                foreach (var record in employeeRecords.Where(r => r.Status == AttendanceStatus.Present && r.ClockOut.HasValue))
                {
                    var clockOutUtc = record.ClockOut.Value;
                    var workingTime = clockOutUtc - record.ClockIn;
                    var breakTime = record.TotalBreakTime;
                    var actualWorkingTime = workingTime - breakTime;
                    totalHours += actualWorkingTime.TotalHours;
                }

                // Handle records that are still active (clocked in but not clocked out)
                foreach (var record in employeeRecords.Where(r => r.Status == AttendanceStatus.Present && !r.ClockOut.HasValue))
                {
                    var workingTime = utcNow - record.ClockIn;
                    var breakTime = record.TotalBreakTime;
                    var actualWorkingTime = workingTime - breakTime;
                    totalHours += actualWorkingTime.TotalHours;
                }

                employeeAttendance.TotalHoursWorked = Math.Round(totalHours, 2);

                report.Employees.Add(employeeAttendance);
            }

            // Sort by employee name
            report.Employees = report.Employees.OrderBy(e => e.EmployeeName).ToList();

            return report;
        }
    }
}
