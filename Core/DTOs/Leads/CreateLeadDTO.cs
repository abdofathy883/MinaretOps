using Core.Enums;
using Core.Models;

namespace Core.DTOs.Leads
{
    public class CreateLeadDTO
    {
        public required string BusinessName { get; set; }
        public required string WhatsAppNumber { get; set; }
        public int ContactAttempts { get; set; }
        public ContactStatus ContactStatus { get; set; } // will be removed
        public LeadSource LeadSource { get; set; }
        public bool DecisionMakerReached { get; set; }
        public bool Interested { get; set; }
        public InterestLevel InterestLevel { get; set; }
        public List<CreateLeadServiceDTO> ServicesInterestedIn { get; set; } = new();
        public bool MeetingAgreed { get; set; }
        public DateTime? MeetingDate { get; set; }
        public MeetingAttend MeetingAttend { get; set; }
        public bool QuotationSent { get; set; }
        public FollowUpReason? FollowUpReason { get; set; }
        public DateTime? FollowUpTime { get; set; }
        public string? Notes { get; set; }
        public string? SalesRepId { get; set; }
        public required string CreatedById { get; set; }
    }
}
