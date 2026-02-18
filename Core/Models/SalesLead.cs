using Core.Enums;

namespace Core.Models
{
    public class SalesLead
    {
        public int Id { get; set; }
        public required string BusinessName { get; set; }
        public required string WhatsAppNumber { get; set; }
        public int ContactAttempts { get; set; }
        public ContactStatus ContactStatus { get; set; }
        public CurrentLeadStatus CurrentLeadStatus { get; set; }
        public LeadSource LeadSource { get; set; }
        public bool DecisionMakerReached { get; set; }
        public bool Interested { get; set; }
        public InterestLevel InterestLevel { get; set; }
        public List<LeadServices> ServicesInterestedIn { get; set; } = new();
        public bool MeetingAgreed { get; set; }
        public DateTime? MeetingDate { get; set; }
        public MeetingAttend MeetingAttend { get; set; }
        public bool QuotationSent { get; set; }
        public DateTime? FollowUpTime { get; set; }
        public FollowUpReason? FollowUpReason { get; set; }
        public string? Notes { get; set; }
        public string? SalesRepId { get; set; }
        public ApplicationUser? SalesRep { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
