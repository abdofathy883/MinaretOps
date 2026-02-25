using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("ServiceCheckpoint", Schema = "Services")]
    public class ServiceCheckpoint
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; } = default!;
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; } // For ordering checkpoints
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
