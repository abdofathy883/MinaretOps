using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Complaint
    {
        public int Id { get; set; }
        public required string Subject { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string EmployeeId { get; set; }
        public ApplicationUser User { get; set; } = default!;
    }
}
