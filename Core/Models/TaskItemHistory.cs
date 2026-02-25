using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("TaskItemHistory", Schema = "Tasks")]
    public class TaskItemHistory
    {
        public int Id { get; set; }
        public int? TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }

        public int? ArchivedTaskId { get; set; }
        public ArchivedTask? ArchivedTask { get; set; }

        public string PropertyName { get; set; } = default!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string? UpdatedById { get; set; }
        public required string UpdatedByName { get; set; }
        public ApplicationUser UpdatedBy { get; set; } = default!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
