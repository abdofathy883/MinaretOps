using Application.DTOs.Portfolio.Item;

namespace Application.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioItemDTO> Create(CreatePortfolioItemDTO createDTO, int? itemId = null);
        Task<List<PortfolioItemDTO>> GetAll();
        Task<PortfolioItemDTO> GetById(int id);
        Task<PortfolioItemDTO> GetBySlug(string slug);
        Task<bool> Delete(int id);
    }
}
