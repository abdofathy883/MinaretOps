namespace Core.DTOs.Tasks
{
    public class TaskGroupDTO
    {
        public int Id { get; set; }
        public int ClientServiceId { get; set; }
        // Month and year for grouping
        public int Month { get; set; } // 1-12
        public int Year { get; set; }
        public string MonthLabel { get; set; } = string.Empty; // e.g., "August 2024"

        public List<TaskDTO> Tasks { get; set; } = new();

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
