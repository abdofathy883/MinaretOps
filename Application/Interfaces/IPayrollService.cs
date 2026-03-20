using Application.DTOs.Salary;

namespace Application.Interfaces
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
        Task<SalaryPeriodDTO> UpdateSalaryPeriod(UpdateSalaryPeriodDTO updateSalary);
    }
}
