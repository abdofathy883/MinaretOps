namespace Core.Models
{
    public class PortfolioCategory
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public List<PortfolioItem> PortfolioItems { get; set; } = new();
    }
}
