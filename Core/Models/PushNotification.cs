using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PushNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // who should see it
        public string Title { get; set; }
        public string Body { get; set; }
        public string Url { get; set; }      // optional link (task detail, etc.)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
