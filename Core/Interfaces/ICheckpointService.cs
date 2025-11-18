using Core.DTOs.Checkpoints;

namespace Core.Interfaces
{
    public interface ICheckpointService
    {
        Task<ServiceCheckpointDTO> AddServiceCheckpointAsync(CreateServiceCheckpointDTO dto);
        Task<List<ServiceCheckpointDTO>> GetServiceCheckpointsAsync(int serviceId);
        Task<ServiceCheckpointDTO> UpdateServiceCheckpointAsync(int checkpointId, UpdateServiceCheckpointDTO dto);
        Task<bool> DeleteServiceCheckpointAsync(int checkpointId);
        Task<List<ClientServiceCheckpointDTO>> GetClientServiceCheckpointsAsync(int clientServiceId);
        Task<ClientServiceCheckpointDTO> MarkCheckpointCompleteAsync(int clientServiceCheckpointId, string employeeId);
        Task<ClientServiceCheckpointDTO> MarkCheckpointIncompleteAsync(int clientServiceCheckpointId);
        Task InitializeClientServiceCheckpointsAsync(int clientServiceId, int serviceId);
    }
}
