using Core.DTOs.KPI;

namespace Core.Interfaces
{
    public interface IKPIService
    {
        Task<IncedintDTO> NewKPIIncedintAsync(CreateIncedintDTO dto);
        Task<EmployeeMonthlyKPIDTO> GetEmployeeMonthlyAsync(string employeeId, int year, int month);
        Task<List<EmployeeMonthlyKPIDTO>> GetMonthlySummeriesAsync(int year, int month);
        Task<List<IncedintDTO>> GetIncedientsAsync(string? employeeId, int year, int month);
    }
}
