using Core.DTOs.Clients;

namespace Core.Interfaces
{
    public interface IClientServices
    {
        Task<List<LightWieghtClientDTO>> GetAllClientsAsync();
        Task<ClientDTO> GetClientByIdAsync(int clientId);
        Task<ClientDTO> AddClientAsync(CreateClientDTO clientDTO, string userId);
        Task<ClientDTO> UpdateClientAsync(int clientId, UpdateClientDTO updateClientDTO);
        Task<bool> DeleteClientAsync(int clientId);
    }
}
