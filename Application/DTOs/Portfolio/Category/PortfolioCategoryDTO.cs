using Application.DTOs.Portfolio.Item;

namespace Application.DTOs.Portfolio.Category
{
    public class PortfolioCategoryDTO
    {
        public int Id { get; set; }
        public List<PortfolioItemDTO> PortfolioItems { get; set; } = new();
        public List<PortfolioCategoryTranslationDTO> Translations { get; set; } = new();
    }
}
