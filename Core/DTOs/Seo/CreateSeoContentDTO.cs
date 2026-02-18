using Microsoft.AspNetCore.Http;

namespace Core.DTOs.Seo
{
    public class CreateSeoContentDTO
    {
        public string Route { get; set; } = string.Empty;
        public string Language { get; set; } = "ar";
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
        public IFormFile? OgImage { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? Robots { get; set; }
    }
}
