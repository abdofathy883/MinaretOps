using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Complaint", Schema = "Communication")]
    public class Complaint
    {
        public int Id { get; set; }
        public required string Subject { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string EmployeeId { get; set; }
        public ApplicationUser User { get; set; } = default!;
    }
}
