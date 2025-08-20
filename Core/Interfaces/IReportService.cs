using Core.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReportService
    {
        Task<List<ClientTaskReportDTO>> GetClientTaskReportAsync(int? clientId = null, int? month = null, int? year = null);
    }
}
