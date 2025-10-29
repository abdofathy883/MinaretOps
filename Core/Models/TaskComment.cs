using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TaskComment
    {
        public int Id { get; set; }
        public required string Comment { get; set; }
        public int TaskId { get; set; }
        public TaskItem Task { get; set; } = default!;
        public string? EmployeeId { get; set; }
        public ApplicationUser? Employee { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
