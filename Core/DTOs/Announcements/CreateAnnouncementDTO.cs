using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Announcements
{
    public class CreateAnnouncementDTO
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
    }
}
