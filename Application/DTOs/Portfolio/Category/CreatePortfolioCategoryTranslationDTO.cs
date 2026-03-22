using Core.Enums;

namespace Application.DTOs.Portfolio.Category
{
    public class CreatePortfolioCategoryTranslationDTO
    {
        public LanguageCode LanguageCode { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}
