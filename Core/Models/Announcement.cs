using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Announcement", Schema = "Communication")]
    public class Announcement
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public List<AnnouncementLink> AnnouncementLinks { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
