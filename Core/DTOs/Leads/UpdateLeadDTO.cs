using Core.Enums;
using Core.Models;

namespace Core.DTOs.Leads
{
    public class UpdateLeadDTO
    {
        public int Id { get; set; }
        public string? BusinessName { get; set; }
        public string? WhatsAppNumber { get; set; }
        public int ContactAttempts { get; set; }
        public ContactStatus ContactStatus { get; set; }
        public LeadSource LeadSource { get; set; }
        public bool DecisionMakerReached { get; set; }
        public bool Interested { get; set; }
        public InterestLevel InterestLevel { get; set; }
        public List<int> ServicesInterestedIn { get; set; } = new();
        public bool MeetingAgreed { get; set; }
        public DateTime? MeetingDate { get; set; }
        public MeetingAttend MeetingAttend { get; set; }
        public bool QuotationSent { get; set; }
        public DateTime? FollowUpTime { get; set; }
        public FollowUpReason FollowUpReason { get; set; }
        public string? Notes { get; set; }
        public string? SalesRepId { get; set; }
    }
}
