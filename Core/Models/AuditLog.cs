namespace Core.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string EntityName { get; set; }   // "User", "Client", "Task"
        public string? Action { get; set; }       // "Created", "Updated", "Deleted"
        public string? EntityId { get; set; }     // PK of the affected row
        public string? OldValues { get; set; }    // JSON snapshot before
        public string? NewValues { get; set; }    // JSON snapshot after
        public string? PerformedBy { get; set; } // UserId from JWT
        public DateTime CreatedAt { get; set; }
    }
}
