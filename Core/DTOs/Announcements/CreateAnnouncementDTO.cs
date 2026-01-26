namespace Core.DTOs.Announcements
{
    public class CreateAnnouncementDTO
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public List<CreateAnnouncementLinkDTO> AnnouncementLinks { get; set; } = new();
    }
}