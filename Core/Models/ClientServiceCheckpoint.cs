using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ClientServiceCheckpoint
    {
        public int Id { get; set; }
        public int ClientServiceId { get; set; }
        public ClientService ClientService { get; set; } = default!;
        public int ServiceCheckpointId { get; set; }
        public ServiceCheckpoint ServiceCheckpoint { get; set; } = default!;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public string? CompletedByEmployeeId { get; set; }
        public ApplicationUser? CompletedByEmployee { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
