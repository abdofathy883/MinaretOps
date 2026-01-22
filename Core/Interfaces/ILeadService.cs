using Core.DTOs.Leads;

namespace Core.Interfaces
{
    public interface ILeadService
    {
        Task<List<LightWieghtLeadDTO>> GetAllLeadsAsync();
        Task<LeadDTO> GetLeadByIdAsync(int id);
        Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO);
        Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO);
        Task<bool> DeleteLeadAsync(Guid leadId);
    }
}
