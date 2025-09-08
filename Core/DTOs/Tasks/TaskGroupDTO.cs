namespace Core.DTOs.Tasks
{
    public class TaskGroupDTO
    {
        public int Id { get; set; }
        public int ClientServiceId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthLabel { get; set; } = string.Empty;
        public List<TaskDTO> Tasks { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
