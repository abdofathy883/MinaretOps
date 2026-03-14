namespace Core.Models
{
    public class LoginLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public bool IsSuccess { get; set; }
        public string? FailureReason { get; set; }
        public string? UserAgent { get; set; }
    }
}
