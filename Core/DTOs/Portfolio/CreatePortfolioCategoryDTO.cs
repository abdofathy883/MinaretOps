namespace Core.DTOs.Portfolio
{
    public class CreatePortfolioCategoryDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}
