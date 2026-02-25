using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("ContactEntry", Schema = "CRM")]
    public class ContactEntry
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public string? Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Message { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSpam { get; set; } = false;
    }
}
