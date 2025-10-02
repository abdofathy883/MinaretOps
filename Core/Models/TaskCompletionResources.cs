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
        public int TaskId { get; set; }
        public TaskItem Task { get; set; } = default!;
        public required string URL { get; set; }
    }
}
