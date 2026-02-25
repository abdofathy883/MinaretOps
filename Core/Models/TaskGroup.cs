using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("TaskGroup", Schema = "Tasks")]
    public class TaskGroup
    {
        public int Id { get; set; }
        public int ClientServiceId { get; set; }
        public ClientService ClientService { get; set; } = default!;
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthLabel { get; set; } = string.Empty;
        public List<TaskItem> Tasks { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
