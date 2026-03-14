namespace Core.DTOs.AuthDTOs
{
    public class LoginLogDto
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public bool IsSuccess { get; set; }
        public string? FailureReason { get; set; }
        public string? UserAgent { get; set; }
    }
}
