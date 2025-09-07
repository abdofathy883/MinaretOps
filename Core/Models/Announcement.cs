using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<EmployeeAnnouncement> EmployeeAnnouncements { get; set; } = new();
    }
}
