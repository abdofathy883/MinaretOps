using Core.DTOs.Leads;
using Core.DTOs.Leads.Notes;

namespace Core.Interfaces
{
    public interface ILeadService
    {
        Task<List<LeadDTO>> GetAllLeadsAsync(string currentUserId);
        Task<LeadDTO> GetLeadByIdAsync(int id);
        Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO, string? currentUserId = null);
        Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO);
        Task<LeadDTO> UpdateLeadFieldAsync(int id, string fieldName, object value);
        Task<bool> DeleteLeadAsync(int leadId);
        Task<List<LeadDTO>> SearchLeadsAsync(string query, string currentUserId);
    }
}
