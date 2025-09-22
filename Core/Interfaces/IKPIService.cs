using Core.DTOs.KPI;

namespace Core.Interfaces
{
    public interface IKPIService
    {
        Task<IncedintDTO> NewKPIIncedintAsync(CreateIncedintDTO dto);
        Task<EmployeeMonthlyKPIDTO> GetEmployeeMonthlyAsync(string employeeId);
        Task<List<EmployeeMonthlyKPIDTO>> GetMonthlySummeriesAsync();
        Task<List<IncedintDTO>> GetIncedientsByEmpIdAsync(string employeeId);
        Task<List<IncedintDTO>> GetAllIncedientsAsync();
    }
}
