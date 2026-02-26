using Core.Enums.Leads;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("SalesLead", Schema = "CRM")]
    public class SalesLead
    {
        public int Id { get; set; }
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
        public List<LeadServices> ServicesInterestedIn { get; set; } = new();
        public List<LeadNote> Notes { get; set; } = new();
        public DateTime? MeetingDate { get; set; }
        public DateTime? FollowUpTime { get; set; }
        //public bool MeetingAgreed { get; set; }
        public bool QuotationSent { get; set; }
        public string? SalesRepId { get; set; }
        public ApplicationUser? SalesRep { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<SalesLeadHistory> LeadHistory { get; set; } = new();
    }
}
