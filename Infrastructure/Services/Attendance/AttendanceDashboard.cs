using Core.DTOs.Attendance.AttendanceDashboard;
using Core.Enums.Auth_Attendance;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Attendance
{
    public class AttendanceDashboard : IAttendanceDashboard
    {
        private readonly MinaretOpsDbContext context;

        public AttendanceDashboard(MinaretOpsDbContext context)
        {
            this.context = context;
        }

        // Add this method to the AttendanceService class

        public async Task<AttendanceDashboardDTO> GetAttendanceDashboardAsync()
        {
            var egyptToday = TimeZoneHelper.GetEgyptToday();
            var egyptNow = TimeZoneHelper.GetEgyptNow();

            // Get all active employees today (clocked in but not clocked out)
            var activeAttendanceRecords = await context.AttendanceRecords
                .Include(r => r.Employee)
                .Include(r => r.BreakPeriods)
                .Where(r => r.WorkDate == egyptToday &&
                           r.Status == AttendanceStatus.Present &&
                           r.ClockOut == null)
                .ToListAsync();

            // Build currently active employees list
            var activeEmployees = new List<ActiveEmployeeDTO>();
            foreach (var record in activeAttendanceRecords)
            {
                // Check if employee has an active break (EndTime is null)
                var isOnBreak = record.BreakPeriods.Any(b => b.EndTime == null);

                activeEmployees.Add(new ActiveEmployeeDTO
                {
                    EmployeeId = record.EmployeeId,
                    EmployeeName = $"{record.Employee.FirstName} {record.Employee.LastName}",
                    IsOnBreak = isOnBreak,
                    ClockInTime = record.ClockIn,
                    WorkingDuration = isOnBreak ? null : egyptNow - record.ClockIn // Only calculate if not on break
                });
            }

            // Get all employees
            var allEmployees = await context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();

            var employeeIdsWithAttendanceToday = activeAttendanceRecords.Select(r => r.EmployeeId).ToList();

            // Check which employees are absent (no attendance record today and it's a working day)
            var absentEmployees = new List<EmployeeDTO>();
            var onLeaveEmployees = new List<EmployeeDTO>();

            foreach (var employee in allEmployees)
            {
                // Skip if employee already clocked in
                if (employeeIdsWithAttendanceToday.Contains(employee.Id))
                    continue;

                // Check if employee has approved leave today
                var hasApprovedLeave = await context.LeaveRequests
                    .AnyAsync(l => l.EmployeeId == employee.Id &&
                                 l.Status == LeaveStatus.Approved &&
                                 l.FromDate.Date <= egyptToday.ToDateTime(TimeOnly.MinValue) &&
                                 l.ToDate.Date >= egyptToday.ToDateTime(TimeOnly.MinValue));

                if (hasApprovedLeave)
                {
                    onLeaveEmployees.Add(new EmployeeDTO
                    {
                        EmployeeId = employee.Id,
                        EmployeeName = $"{employee.FirstName} {employee.LastName}"
                    });
                }
                else
                {
                    // Check if it's a working day (not Friday)
                    var todayIsWorkingDay = egyptToday.ToDateTime(TimeOnly.MinValue).DayOfWeek != DayOfWeek.Friday;

                    if (todayIsWorkingDay)
                    {
                        absentEmployees.Add(new EmployeeDTO
                        {
                            EmployeeId = employee.Id,
                            EmployeeName = $"{employee.FirstName} {employee.LastName}"
                        });
                    }
                }
            }

            return new AttendanceDashboardDTO
            {
                CurrentlyActiveTotal = activeEmployees.Count,
                CurrentlyActiveEmployees = activeEmployees.OrderBy(e => e.EmployeeName).ToList(),

                AbsentTotal = absentEmployees.Count,
                AbsentEmployees = absentEmployees.OrderBy(e => e.EmployeeName).ToList(),

                OnLeaveTotal = onLeaveEmployees.Count,
                OnLeaveEmployees = onLeaveEmployees.OrderBy(e => e.EmployeeName).ToList()
            };
        }
    }
}
