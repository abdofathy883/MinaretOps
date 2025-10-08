using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Outbox
    {
        public int Id { get; set; }
        public OutboxTypes OpType { get; set; }
        public required string OpTitle { get; set; }
        public required string PayLoad { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }
}
