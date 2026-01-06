using Core.DTOs.Salary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPayrollService
    {
        Task<SalaryPeriodDTO> CreateSalaryPeriodAsync(CreateSalaryPeriodDTO createSalaryPeriodDTO);
        Task<SalaryPaymentDTO> RecordSalaryPaymentAsync(CreateSalaryPaymentDTO createSalaryPaymentDTO);
        Task<SalaryPeriodDTO?> GetSalaryPeriodAsync(string employeeId, int month, int year);
        Task<List<SalaryPeriodDTO>> GetSalaryPeriodsAsync(string employeeId);
        Task<List<SalaryPeriodDTO>> GetAllSalaryPeriodsAsync();
        Task<SalaryPeriodDTO?> GetSalaryPeriodByIdAsync(int periodId);
        Task<List<SalaryPaymentDTO>> GetSalaryPaymentsAsync(string employeeId);
    }
}
