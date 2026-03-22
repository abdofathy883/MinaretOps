using Core.Enums;

namespace Application.DTOs.Portfolio.Category
{
    public class PortfolioCategoryTranslationDTO
    {
        public int Id { get; set; }
        public LanguageCode LanguageCode { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
    }
}
