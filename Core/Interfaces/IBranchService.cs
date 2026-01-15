using Core.DTOs.Branch;

namespace Core.Interfaces
{
    public interface IBranchService
    {
        Task<List<BranchDTO>> GetAllAsync();
        Task<BranchDTO> GetByIdAsync(int id);
        Task<BranchDTO> CreateAsync(CreateBranchDTO createBranchDTO);
        Task<BranchDTO> UpdateAsync(int id, UpdateBranchDTO updateBranchDTO);
        Task<bool> DeleteAsync(int id);
    }
}
