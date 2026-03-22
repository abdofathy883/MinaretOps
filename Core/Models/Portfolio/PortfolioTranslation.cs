using Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Portfolio
{
    [Table("PortfolioTranslation", Schema = "Content")]
    public class PortfolioTranslation
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? ImageAltText { get; set; }
        public LanguageCode LanguageCode { get; set; }
        public PublicContentStatus Status { get; set; }
        public int PortfolioItemId { get; set; }
        public PortfolioItem PortfolioItem { get; set; } = default!;
    }
}
