namespace Application.DTOs.Portfolio
{
    public class PortfolioCategoryDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public List<PortfolioItemDTO> PortfolioItems { get; set; } = new();
    }
}
