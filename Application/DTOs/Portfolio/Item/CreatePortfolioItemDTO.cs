using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Portfolio.Item
{
    public class CreatePortfolioItemDTO
    {
        public string? Slug { get; set; }
        public IFormFile? ImageFile { get; set; }
        public int? CategoryId { get; set; }
        public List<CreatePortfolioItemTranslationDTO> Translations { get; set; } = new();
    }
}
