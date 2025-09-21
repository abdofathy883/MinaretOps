namespace Core.DTOs.InternalTasks
{
    public class InternalTaskAssignmentDTO
    {
        public int Id { get; set; }
        public int InternalTaskId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool IsLeader { get; set; }
    }
}
