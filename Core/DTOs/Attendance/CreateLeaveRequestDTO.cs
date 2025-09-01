using Core.Enums;

namespace Core.DTOs.Attendance
{
    public class CreateLeaveRequestDTO
    {
        public required string EmployeeId { get; set; }
        public DateTime Date { get; set; }
        //public string Reason { get; set; }

        public LeaveStatus Status { get; set; }
        public string AdminId { get; set; }     // who approved/rejected
        public DateTime? ActionDate { get; set; }
    }
}
