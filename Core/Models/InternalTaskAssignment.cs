using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("InternalTaskAssignment", Schema = "Tasks")]
    public class InternalTaskAssignment
    {
        public int Id { get; set; }
        public int InternalTaskId { get; set; }
        public virtual InternalTask Task { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool IsLeader { get; set; }
    }
}
