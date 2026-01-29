using Core.DTOs.Leads;

namespace Core.Interfaces
{
    public interface ILeadService
    {
        Task<List<LeadDTO>> GetAllLeadsAsync();
        Task<LeadDTO> GetLeadByIdAsync(int id);
        Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO);
        Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO);
        Task<LeadDTO> UpdateLeadFieldAsync(int id, string fieldName, object value);
        Task<bool> DeleteLeadAsync(int leadId);
    }
}
