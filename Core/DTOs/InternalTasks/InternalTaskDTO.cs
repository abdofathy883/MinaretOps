using Core.Enums;

namespace Core.DTOs.InternalTasks
{
    public class InternalTaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public required InternalTaskType TaskType { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public CustomTaskStatus Status { get; set; }
        public string Priority { get; set; }
        public bool IsCompletedOnDeadline =>
            Status == CustomTaskStatus.Completed &&
            CompletedAt.HasValue &&
            CompletedAt.Value.Date <= Deadline.Date;
        public List<InternalTaskAssignmentDTO> Assignments { get; set; }
    }
}
