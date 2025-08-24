namespace Core.Models
{
    public class TaskGroup
    {
        public int Id { get; set; }
        public int ClientServiceId { get; set; }
        public ClientService ClientService { get; set; } = default!;

        // Month and year for grouping
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthLabel { get; set; } = string.Empty; // e.g., "August 2024"

        public List<TaskItem> Tasks { get; set; } = new();

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
