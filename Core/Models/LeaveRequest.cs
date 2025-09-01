using Core.Enums;

namespace Core.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; } = default!;
        public DateTime Date { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime? ActionDate { get; set; }
    }
}
