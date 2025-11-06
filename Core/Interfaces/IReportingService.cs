using Core.DTOs.Reporting;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReportingService
    {
        Task<TaskEmployeeReportDTO> GetTaskEmployeeReportAsync(string currentUserId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<MonthlyAttendanceReportDTO> GetMonthlyAttendanceReportAsync(DateTime fromDate, DateTime toDate, AttendanceStatus? status = null);
    }
}
