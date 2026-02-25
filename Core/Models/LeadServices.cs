using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("LeadServices", Schema = "CRM")]
    public class LeadServices
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; } = default!;

        public int LeadId { get; set; }
        public SalesLead Lead { get; set; } = default!;
    }
}
