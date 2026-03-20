using Application.DTOs.Services;

namespace Application.Interfaces
{
    public interface IServiceService
    {
        Task<ServiceDTO> AddServiceAsync(CreateServiceDTO newService);
        Task<ServiceDTO> UpdateServiceAsync(UpdateServiceDTO serviceDTO);
        Task<ServiceDTO> ToggleVisibilityAsync(int serviceId);
        Task<bool> DeleteServiceAsync(int serviceId);
        Task<List<ServiceDTO>> GetAllServicesAsync();
        Task<ServiceDTO> GetServiceByIdAsync(int serviceId);
    }
}
