using Core.DTOs.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReportingService
    {
        Task<TaskEmployeeReportDTO> GetTaskEmployeeReportAsync(string currentUserId);
    }
}
