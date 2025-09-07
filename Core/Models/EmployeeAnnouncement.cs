using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class EmployeeAnnouncement
    {
        public string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; } = default!;

        public int AnnouncementId { get; set; }
        public Announcement Announcement { get; set; } = default!;
        public bool IsRead { get; set; }
    }
}
