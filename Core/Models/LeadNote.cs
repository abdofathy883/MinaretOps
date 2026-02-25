using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("LeadNote", Schema = "CRM")]
    public class LeadNote
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public required string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; } = default!;
        public int LeadId { get; set; }
        public SalesLead Lead { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
