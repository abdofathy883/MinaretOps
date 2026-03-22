using Core.Enums;

namespace Application.DTOs.Portfolio.Item
{
    public class PortfolioTranslationDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? ImageAltText { get; set; }
        public LanguageCode LanguageCode { get; set; }
        public PublicContentStatus Status { get; set; }
        public int PortfolioItemId { get; set; }
    }
}
