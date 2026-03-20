using Application.DTOs.Leads;

namespace Application.DTOs.Leads.Reports
{
    public class LeadEmployeeReportDTO
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        
        public int TotalAssignedLeads { get; set; }
        public int MeetingAgreedCount { get; set; }
        public int QuotationSentCount { get; set; }
        public int DealCount { get; set; }

        public List<LeadDTO> Leads { get; set; } = new();
    }
}
