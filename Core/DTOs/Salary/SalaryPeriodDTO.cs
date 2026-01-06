using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Salary
{
    public class SalaryPeriodDTO
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public required string MonthLabel { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal Salary { get; set; }
        public decimal Bonus { get; set; } = 0;
        public decimal Deductions { get; set; } = 0;

        public decimal DueAmount => Salary + Bonus - Deductions;
        public decimal TotalPaidAmount => SalaryPayments?.Sum(p => p.Amount) ?? 0;
        public decimal RemainingAmount => Math.Max(0, DueAmount - TotalPaidAmount);
        public List<SalaryPaymentDTO> SalaryPayments { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }
    }
}
