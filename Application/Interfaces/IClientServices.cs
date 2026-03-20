using Application.DTOs.Clients;
using Core.Enums;

namespace Application.Interfaces
{
    public interface IClientServices
    {
        Task<List<LightWieghtClientDTO>> GetAllAsync(ClientStatus status);
        Task<List<LightWieghtClientDTO>> SearchAsync(string clientName);
        Task<List<LightWieghtClientDTO>> GetAllActiveAsync();
        Task<ClientDTO> GetClientByIdAsync(int clientId);
        Task<ClientDTO> AddClientAsync(CreateClientDTO clientDTO, string userId);
        Task<ClientDTO> UpdateClientAsync(int clientId, UpdateClientDTO updateClientDTO);
        Task<bool> DeleteClientAsync(int clientId);
    }
}
