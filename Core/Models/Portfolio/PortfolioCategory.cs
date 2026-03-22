using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Portfolio
{
    [Table("PortfolioCategory", Schema = "Content")]
    public class PortfolioCategory
    {
        public int Id { get; set; }
        public List<PortfolioItem> PortfolioItems { get; set; } = new();
        public List<PortfolioCategoryTranslation> Translations { get; set; } = new();
    }
}
