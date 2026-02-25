using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("SeoContent", Schema = "Content")]
    public class SeoContent
    {
        public int Id { get; set; }
        public required string Route { get; set; }
        public string Language { get; set; } = "ar";
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
        public string? OgImage { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? Robots { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
