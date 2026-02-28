using Microsoft.AspNetCore.Http;

namespace Core.DTOs.Portfolio
{
    public class CreatePortfolioItemDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageAltText { get; set; }
        public int? CategoryId { get; set; }
    }
}
