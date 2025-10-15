namespace Core.Models
{
    public class TaskItemHistory
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = default!;

        public string PropertyName { get; set; } = default!; // e.g. "Status", "Deadline"
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string UpdatedById { get; set; } = default!;
        public ApplicationUser UpdatedBy { get; set; } = default!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
