namespace Core.DTOs.Salary
{
    public class CreateSalaryPaymentDTO
    {
        public required string EmployeeId { get; set; }
        public int? SalaryPeriodId { get; set; }
        public int VaultId { get; set; }
        public int CurrencyId { get; set; }
        public string CreatedBy { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
    }
}
