using Core.Enums;

namespace Core.DTOs.Tasks.TaskDTOs
{
    public class UpdateTaskDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public CustomTaskStatus Status { get; set; }
        public required string Priority { get; set; }
        public string? Refrence { get; set; }
        public required string EmployeeId { get; set; }
        public int? NumberOfSubTasks { get; set; }
    }
}
