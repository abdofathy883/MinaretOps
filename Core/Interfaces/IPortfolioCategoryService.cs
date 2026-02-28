using Core.DTOs.Portfolio;

namespace Core.Interfaces
{
    public interface IPortfolioCategoryService
    {
        Task<PortfolioCategoryDTO> Create(CreatePortfolioCategoryDTO createCategory);
        Task<PortfolioCategoryDTO> GetAll();
        Task<PortfolioCategoryDTO> GetById(int id);
    }
}
