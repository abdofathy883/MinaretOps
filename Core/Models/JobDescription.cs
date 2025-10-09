using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class JobDescription
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public IdentityRole? Role { get; set; }
        public List<JobResponsibility> JobResponsibilities { get; set; } = new();
    }
}
