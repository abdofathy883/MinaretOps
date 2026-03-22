using Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Portfolio
{
    [Table("PortfolioCategoryTranslation", Schema = "Content")]
    public class PortfolioCategoryTranslation
    {
        public int Id { get; set; }
        public LanguageCode LanguageCode { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public PortfolioCategory PortfolioCategory { get; set; } = default!;
    }
}
