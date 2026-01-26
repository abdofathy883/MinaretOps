using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Announcements
{
    public class AnnouncementLinkDTO
    {
        public int Id { get; set; }
        public required string Link { get; set; }
    }
}
