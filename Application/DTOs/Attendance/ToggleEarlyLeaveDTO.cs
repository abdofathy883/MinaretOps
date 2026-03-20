namespace Application.DTOs.Attendance
{
    public class ToggleEarlyLeaveDTO
    {
        public required string EmployeeId { get; set; }
        public required DateOnly WorkDate { get; set; }
    }
}
