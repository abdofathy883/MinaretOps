using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("SalesLeadHistory", Schema = "CRM")]
    public class SalesLeadHistory
    {
        public int Id { get; set; }
        public int SalesLeadId { get; set; }
        public SalesLead SalesLead { get; set; } = default!;

        public string PropertyName { get; set; } = default!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string? UpdatedById { get; set; }
        public required string UpdatedByName { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
