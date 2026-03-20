using Application.DTOs.Portfolio;

namespace Application.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioItemDTO> Create(CreatePortfolioItemDTO createDTO);
        Task<List<PortfolioItemDTO>> GetAll();
        Task<PortfolioItemDTO> GetById(int id);
        //Task<PortfolioItemDTO> GetByTitle(string title);
        Task<bool> Delete(int id);
    }
}
