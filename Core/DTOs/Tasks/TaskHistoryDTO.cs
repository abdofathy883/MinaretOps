using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Tasks
{
    public class TaskHistoryDTO
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public string PropertyName { get; set; } = default!; // e.g. "Status", "Deadline"
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string UpdatedById { get; set; } = default!;
        public string UpdatedByName { get; set; } = default!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
