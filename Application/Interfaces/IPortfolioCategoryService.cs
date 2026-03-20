using Application.DTOs.Portfolio;

namespace Application.Interfaces
{
    public interface IPortfolioCategoryService
    {
        Task<PortfolioCategoryDTO> Create(CreatePortfolioCategoryDTO createCategory);
        Task<List<PortfolioCategoryDTO>> GetAll();
        Task<PortfolioCategoryDTO> GetById(int id);
    }
}
