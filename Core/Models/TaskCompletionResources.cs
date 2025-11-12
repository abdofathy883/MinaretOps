using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TaskCompletionResources
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }  // Changed from int to int? (nullable)
        public TaskItem? Task { get; set; }  // Changed from default! to nullable

        public int? ArchivedTaskId { get; set; }  // ADD THIS
        public ArchivedTask? ArchivedTask { get; set; }  // ADD THIS
        public required string URL { get; set; }
    }
}
