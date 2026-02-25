using Core.DTOs.Leads.Notes;
using Core.Enums.Leads;

namespace Core.DTOs.Leads
{
    public class UpdateLeadDTO
    {
        public int Id { get; set; }
        public string? BusinessName { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Country { get; set; }
        public string? Occupation { get; set; }
        public ContactStatus ContactStatus { get; set; }
        public CurrentLeadStatus CurrentLeadStatus { get; set; }
        public LeadSource LeadSource { get; set; }
        public InterestLevel InterestLevel { get; set; }
        public FreelancePlatform? FreelancePlatform { get; set; }
        public LeadResponsibility Responsibility { get; set; }
        public LeadBudget Budget { get; set; }
        public LeadTimeline Timeline { get; set; }
        public NeedsExpectation NeedsExpectation { get; set; }
        public List<int> ServicesInterestedIn { get; set; } = new();
        public bool QuotationSent { get; set; }
        public DateTime? MeetingDate { get; set; }
        public DateTime? FollowUpTime { get; set; }
        public string? SalesRepId { get; set; }
    }
}
