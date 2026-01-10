namespace Core.DTOs.Salary
{
    public class UpdateSalaryPeriodDTO
    {
        public int Id { get; set; }
        public decimal Bonus { get; set; } = 0;
        public decimal Deductions { get; set; } = 0;
        public string? Notes { get; set; }
    }
}
