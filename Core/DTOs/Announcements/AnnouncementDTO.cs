namespace Core.DTOs.Announcements
{
    public class AnnouncementDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public List<AnnouncementLinkDTO> AnnouncementLinks { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
