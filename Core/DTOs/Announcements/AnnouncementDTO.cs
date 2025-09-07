namespace Core.DTOs.Announcements
{
    public class AnnouncementDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
