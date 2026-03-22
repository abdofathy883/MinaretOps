using Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Portfolio
{
    [Table("PortfolioItem", Schema = "Content")]
    public class PortfolioItem
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public DateTime PublishedAt { get; set; }
        public string? ImageLink { get; set; }
        public int? CategoryId { get; set; }
        public PortfolioCategory? Category { get; set; }
        public List<PortfolioTranslation> Translations { get; set; } = new();
    }
}
