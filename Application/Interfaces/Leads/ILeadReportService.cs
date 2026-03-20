using Application.DTOs.Leads.Reports;

namespace Application.Interfaces.Leads
{
    public interface ILeadReportService
    {
        Task<List<LeadEmployeeReportDTO>> GetLeadsEmployeeReportAsync(string currentUserId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
