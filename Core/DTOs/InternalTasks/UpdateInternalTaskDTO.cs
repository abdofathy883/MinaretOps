using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.InternalTasks
{
    public class UpdateInternalTaskDTO
    {
        public string Title { get; set; }
        public required InternalTaskType TaskType { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public CustomTaskStatus Status { get; set; }
        public string Priority { get; set; }
        public List<CreateInternalTaskAssignmentDTO> Assignments { get; set; }
    }
}
