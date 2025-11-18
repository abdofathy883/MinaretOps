using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Checkpoints
{
    public class ServiceCheckpointDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
