using Core.DTOs.Leads.Notes;
using Core.Enums.Leads;

namespace Core.DTOs.Leads
{
    public class CreateLeadDTO
    {
        public required string BusinessName { get; set; }
        public required string WhatsAppNumber { get; set; }
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
        public List<CreateLeadServiceDTO> ServicesInterestedIn { get; set; } = new();
        public DateTime? MeetingDate { get; set; }
        public DateTime? FollowUpTime { get; set; }
        //public bool MeetingAgreed { get; set; }
        public bool QuotationSent { get; set; }
        public List<CreateLeadNoteDTO> Notes { get; set; } = new();
        public string? SalesRepId { get; set; }
        public string? CreatedById { get; set; }
    }
}
