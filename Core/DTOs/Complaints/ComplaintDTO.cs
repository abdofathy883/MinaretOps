namespace Core.DTOs.Complaints
{
    public class ComplaintDTO
    {
        public int Id { get; set; }
        public required string Subject { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }
}
