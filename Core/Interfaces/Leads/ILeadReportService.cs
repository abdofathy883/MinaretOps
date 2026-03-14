using Core.DTOs.Leads.Reports;

namespace Core.Interfaces.Leads
{
    public interface ILeadReportService
    {
        Task<List<LeadEmployeeReportDTO>> GetLeadsEmployeeReportAsync(string currentUserId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
