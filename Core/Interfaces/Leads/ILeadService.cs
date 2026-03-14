using Core.DTOs.Leads;
using Core.DTOs.Leads.Notes;

namespace Core.Interfaces.Leads
{
    public interface ILeadService
    {
        Task<PaginatedLeadResultDTO> GetAllLeadsAsync(LeadFilterDTO filter);
        Task<LeadDTO> GetLeadByIdAsync(int id);
        Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO, string? currentUserId = null);
        Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO);
        Task<LeadDTO> UpdateLeadFieldAsync(int id, string fieldName, object value);
        Task<bool> DeleteLeadAsync(int leadId);
        Task<List<LeadDTO>> SearchLeadsAsync(string query, string currentUserId);
    }
}
