using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Salary
{
    public class CreateSalaryPaymentDTO
    {
        public required string EmployeeId { get; set; }
        public int? SalaryPeriodId { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
    }
}
