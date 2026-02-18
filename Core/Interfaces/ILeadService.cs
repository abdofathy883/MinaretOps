using Core.DTOs.Leads;

namespace Core.Interfaces
{
    public interface ILeadService
    {
        Task<List<LeadDTO>> GetAllLeadsAsync(string currentUserId);
        Task<List<LeadDTO>> SearchLeadsAsync(string query, string currentUserId);
        Task<LeadDTO> GetLeadByIdAsync(int id);
        Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO, string? currentUserId = null);
        Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO);
        Task<LeadDTO> UpdateLeadFieldAsync(int id, string fieldName, object value);
        Task<bool> DeleteLeadAsync(int leadId);
        Task ImportLeadsFromExcelAsync(Stream fileStream, string currentUserId);
        Task<byte[]> ExportLeadsToExcelAsync(string userId);
        Task<byte[]> GetLeadTemplateAsync();
    }
}
