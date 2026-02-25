using Core.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Service", Schema = "Services")]
    public class Service: IAuditable, IDeletable
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public List<ClientService> ClientServices { get; set; } = new();
        public List<ServiceCheckpoint> ServiceCheckpoints { get; set; } = new();
        public List<LeadServices> LeadServices { get; set; } = new();
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
