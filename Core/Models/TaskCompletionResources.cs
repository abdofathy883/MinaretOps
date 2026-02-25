using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("TaskCompletionResources", Schema = "Tasks")]
    public class TaskCompletionResources
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public TaskItem? Task { get; set; }

        public int? ArchivedTaskId { get; set; }
        public ArchivedTask? ArchivedTask { get; set; }
        public required string URL { get; set; }
    }
}
