namespace Core.Models
{
    public class SalaryPayment
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; } = default!;
        public int? SalaryPeriodId { get; set; }
        public SalaryPeriod? SalaryPeriod { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
