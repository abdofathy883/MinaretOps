namespace Core.DTOs.Complaints
{
    public class CreateComplaintDTO
    {
        public required string Subject { get; set; }
        public required string Content { get; set; }
        public required string EmployeeId { get; set; }
    }
}
