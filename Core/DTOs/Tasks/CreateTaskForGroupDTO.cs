using Core.Enums;

namespace Core.DTOs.Tasks
{
    public class CreateTaskForGroupDTO
    {
        public required string Title { get; set; }
        public required TaskType TaskType { get; set; }
        public required string Description { get; set; }
        public CustomTaskStatus Status { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "عادي";
        public string? Refrence { get; set; }
        public string EmployeeId { get; set; }
    }
}
