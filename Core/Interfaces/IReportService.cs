using Core.DTOs.Reports;

namespace Core.Interfaces
{
    public interface IReportService
    {
        Task<List<ClientTaskReportDTO>> GetClientTaskReportAsync(int? clientId = null, int? month = null, int? year = null);
    }
}
