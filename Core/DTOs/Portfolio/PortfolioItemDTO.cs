namespace Core.DTOs.Portfolio
{
    public class PortfolioItemDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? ImageLink { get; set; }
        public string? ImageAltText { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
