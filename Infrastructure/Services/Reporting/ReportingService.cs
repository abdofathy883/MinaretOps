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

        public async Task<TaskEmployeeReportDTO> GetTaskEmployeeReportAsync(string currentUserId)
        {
            var egyptToday = TimeZoneHelper.GetEgyptToday();
            var egyptNow = TimeZoneHelper.GetEgyptNow();
            var todayStart = egyptToday.ToDateTime(TimeOnly.MinValue);
            var todayEnd = egyptToday.ToDateTime(TimeOnly.MaxValue);

            // Get attendance data for today
            var activeAttendanceRecords = await context.AttendanceRecords
                .Include(r => r.Employee)
                .Include(r => r.BreakPeriods)
                .Where(r => r.WorkDate == egyptToday && 
                r.Status == AttendanceStatus.Present &&
                r.ClockOut == null)
                .ToListAsync();

            // Get all employees
            var allEmployees = await context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();

            var employeeIdsWithAttendanceToday = activeAttendanceRecords
                .Select(r => r.EmployeeId)
                .ToList();

            // Separate working and on-break employees
            var workingEmployees = new List<EmployeeWithTasksDTO>();
            var onBreakEmployees = new List<EmployeeWithTasksDTO>();

            foreach (var record in activeAttendanceRecords)
            {
                var isOnBreak = record.BreakPeriods.Any(b => b.EndTime == null);
                var employeeInfo = new EmployeeWithTasksDTO
                {
                    EmployeeId = record.EmployeeId,
                    EmployeeName = $"{record.Employee.FirstName} {record.Employee.LastName}",
                    IsOnBreak = isOnBreak,
                    ClockInTime = record.ClockIn,
                    WorkingDuration = isOnBreak ? null : egyptNow - record.ClockIn
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

            // Get absent employees
            var absentEmployees = new List<EmployeeWithTasksDTO>();
            foreach (var employee in allEmployees)
            {
                if (employeeIdsWithAttendanceToday.Contains(employee.Id))
                    continue;

                // Check if employee has approved leave today
                var hasApprovedLeave = await context.LeaveRequests
                    .AnyAsync(l => l.EmployeeId == employee.Id &&
                                 l.Status == LeaveStatus.Approved &&
                                 l.FromDate.Date <= egyptToday.ToDateTime(TimeOnly.MinValue) &&
                                 l.ToDate.Date >= egyptToday.ToDateTime(TimeOnly.MinValue));

                if (!hasApprovedLeave)
                {
                    // Check if it's a working day (not Friday)
                    var todayIsWorkingDay = egyptToday.ToDateTime(TimeOnly.MinValue).DayOfWeek != DayOfWeek.Friday;
                    if (todayIsWorkingDay)
                    {
                        absentEmployees.Add(new EmployeeWithTasksDTO
                        {
                            EmployeeId = employee.Id,
                            EmployeeName = $"{employee.FirstName} {employee.LastName}"
                        });
                    }
                }
            }

            // Get current user for role-based filtering
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
                throw new Exception("User not found");

            var roles = await userManager.GetRolesAsync(currentUser);

            // Helper function to get tasks for an employee
            async Task<List<LightWieghtTaskDTO>> GetTasksForEmployeeAsync(string employeeId)
            {
                IQueryable<TaskItem> query = context.Tasks
                    .Include(t => t.ClientService)
                        .ThenInclude(cs => cs.Service)
                    .Include(t => t.ClientService)
                        .ThenInclude(cs => cs.Client)
                    .Include(t => t.Employee)
                    .Where(t => t.EmployeeId == employeeId &&
                               t.Deadline >= todayStart &&
                               t.Status != CustomTaskStatus.Completed);

                // Apply role-based filtering
                if (roles.Contains(UserRoles.Admin.ToString()) || roles.Contains(UserRoles.AccountManager.ToString()))
                {
                    // Return all tasks (no empId filter needed)
                }
                else if (roles.Contains("ContentCreatorTeamLeader"))
                {
                    var contentTypes = new[] { TaskType.ContentWriting, TaskType.ContentStrategy };
                    query = query.Where(t => contentTypes.Contains(t.TaskType));
                }
                else if (roles.Contains("GraphicDesignerTeamLeader"))
                {
                    var contentTypes = new[]
                    {
                TaskType.Illustrations,
                TaskType.LogoDesign,
                TaskType.VisualIdentity,
                TaskType.DesignDirections,
                TaskType.SM_Design,
                TaskType.Motion,
                TaskType.VideoEditing
            };
                    query = query.Where(t => contentTypes.Contains(t.TaskType));
                }
                else
                {
                    // Default: return only tasks assigned to this employee
                    // Already filtered by employeeId above
                }

                // Filter: deadline not passed and not completed
                var currentDate = DateTime.UtcNow;
                query = query.Where(t => t.Deadline >= currentDate && t.Status != CustomTaskStatus.Completed);

                var tasks = await query.ToListAsync();
                return mapper.Map<List<LightWieghtTaskDTO>>(tasks);
            }

            // Load tasks for all employees in parallel
            var allEmployeeIds = workingEmployees.Select(e => e.EmployeeId)
                .Concat(onBreakEmployees.Select(e => e.EmployeeId))
                .Concat(absentEmployees.Select(e => e.EmployeeId))
                .ToList();

            var taskLoadingTasks = allEmployeeIds.Select(async employeeId =>
            {
                var tasks = await GetTasksForEmployeeAsync(employeeId);
                return new { EmployeeId = employeeId, Tasks = tasks };
            });

            var taskResults = await Task.WhenAll(taskLoadingTasks);
            var taskDictionary = taskResults.ToDictionary(x => x.EmployeeId, x => x.Tasks);

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
    }
}
