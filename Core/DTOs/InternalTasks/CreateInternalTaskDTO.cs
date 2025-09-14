using Core.Enums;

namespace Core.DTOs.InternalTasks
{
    public class CreateInternalTaskDTO
    {
        public string Title { get; set; }
        public required InternalTaskType TaskType { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public List<CreateInternalTaskAssignmentDTO> Assignments { get; set; }
    }
}
