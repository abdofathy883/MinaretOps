using Core.Models.Portfolio;

namespace Application.DTOs.Portfolio.Item
{
    public class PortfolioItemDTO
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public DateTime PublishedAt { get; set; }
        public string? ImageLink { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<PortfolioTranslationDTO> Translations { get; set; } = new();
    }
}
