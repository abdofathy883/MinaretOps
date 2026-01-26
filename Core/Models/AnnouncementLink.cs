namespace Core.Models
{
    public class AnnouncementLink
    {
        public int Id { get; set; }
        public required string Link { get; set; }
        public int AnnouncementId { get; set; }
        public Announcement Announcement { get; set; } = default!;
    }
}
