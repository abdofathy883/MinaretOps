using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("AnnouncementLink", Schema = "Communication")]
    public class AnnouncementLink
    {
        public int Id { get; set; }
        public required string Link { get; set; }
        public int AnnouncementId { get; set; }
        public Announcement Announcement { get; set; } = default!;
    }
}
