using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Salary
{
    public class CreateSalaryPeriodDTO
    {
        public required string EmployeeId { get; set; }

        public decimal Salary { get; set; }
        public decimal Bonus { get; set; } = 0;
        public decimal Deductions { get; set; } = 0;
        public List<CreateSalaryPaymentDTO> SalaryPayments { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}
