using Core.Enums;

namespace Core.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; } = default!;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public LeaveRequestType Type { get; set; }
        public required string Reason { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ActionDate { get; set; }
        public string? ProofFile { get; set; }
    }
}
