using Application.DTOs.Portfolio.Category;

namespace Application.Interfaces
{
    public interface IPortfolioCategoryService
    {
        Task<PortfolioCategoryDTO> Create(CreatePortfolioCategoryDTO createCategory, int? categoryId);
        Task<List<PortfolioCategoryDTO>> GetAll();
        Task<PortfolioCategoryDTO> GetById(int id);
        Task<bool> Delete(int id);
    }
}
