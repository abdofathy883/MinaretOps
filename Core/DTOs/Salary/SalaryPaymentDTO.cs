namespace Core.DTOs.Salary
{
    public class SalaryPaymentDTO
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public int? SalaryPeriodId { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
