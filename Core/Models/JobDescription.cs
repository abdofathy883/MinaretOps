using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("JobDescription", Schema = "HR")]
    public class JobDescription
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public IdentityRole? Role { get; set; }
        public List<JobResponsibility> JobResponsibilities { get; set; } = new();
    }
}
