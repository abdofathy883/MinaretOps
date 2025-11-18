using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Checkpoints
{
    public class ClientServiceCheckpointDTO
    {
        public int Id { get; set; }
        public int ClientServiceId { get; set; }
        public int ServiceCheckpointId { get; set; }
        public string ServiceCheckpointName { get; set; } = string.Empty;
        public string? ServiceCheckpointDescription { get; set; }
        public int ServiceCheckpointOrder { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CompletedByEmployeeId { get; set; }
        public string? CompletedByEmployeeName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
