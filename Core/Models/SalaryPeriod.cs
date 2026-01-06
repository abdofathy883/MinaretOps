namespace Core.Models
{
    public class SalaryPeriod
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; } = default!;

        // Period identification (e.g., "2024-01" for January 2024)
        public required string MonthLabel { get; set; } // Format: "YYYY-MM"
        public int Month { get; set; }
        public int Year { get; set; }

        // Salary components for this period
        public decimal Salary { get; set; }
        public decimal Bonus { get; set; } = 0;
        public decimal Deductions { get; set; } = 0;

        // Computed property: Total due for this period
        public decimal DueAmount => Salary + Bonus - Deductions;

        // Computed property: Total paid for this period
        public decimal TotalPaidAmount => SalaryPayments?.Sum(p => p.Amount) ?? 0;

        // Computed property: Remaining amount for this period
        public decimal RemainingAmount => Math.Max(0, DueAmount - TotalPaidAmount);

        // Navigation property to payments for this period
        public List<SalaryPayment> SalaryPayments { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Optional: Notes about this period
        public string? Notes { get; set; }
    }
}
