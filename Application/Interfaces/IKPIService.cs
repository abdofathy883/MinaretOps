using Application.DTOs.KPI;

namespace Application.Interfaces
{
    public interface IKPIService
    {
        Task<IncedintDTO> NewKPIIncedintAsync(CreateIncedintDTO dto);
        Task<EmployeeMonthlyKPIDTO> GetEmployeeMonthlyAsync(string employeeId, int? month = null, int? year = null);
        Task<List<EmployeeMonthlyKPIDTO>> GetMonthlySummeriesAsync(int? month = null, int? year = null);
        Task<List<IncedintDTO>> GetIncedientsByEmpIdAsync(string employeeId, int? month = null, int? year = null);
        Task<List<IncedintDTO>> GetAllIncedientsAsync();
    }
}
