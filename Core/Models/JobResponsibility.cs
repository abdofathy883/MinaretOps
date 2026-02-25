using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("JobResponsibility", Schema = "HR")]
    public class JobResponsibility
    {
        public int Id { get; set; }
        public int JobDescriptionId { get; set; }
        public JobDescription JobDescription { get; set; } = default!;
        public required string Text { get; set; }
    }
}
