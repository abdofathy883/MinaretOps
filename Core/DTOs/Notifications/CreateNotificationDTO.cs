using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Notifications
{
    public class CreateNotificationDTO
    {
        public string UserId { get; set; }   // who should see it
        public string Title { get; set; }
        public string Body { get; set; }
        public string Url { get; set; }
    }
}
