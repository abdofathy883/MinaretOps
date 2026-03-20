using Application.DTOs.Contract;

namespace Application.Interfaces
{
    public interface IContractService
    {
        Task<List<ContractDTO>> GetAll();
        Task<ContractDTO> Create(CreateContractDTO contractDTO, string currentUserId);
        Task<ContractDTO> GetById(int id);
        Task<ContractDTO> Update(UpdateContract updateContract);
        Task<bool> Delete(int id);
    }
}
