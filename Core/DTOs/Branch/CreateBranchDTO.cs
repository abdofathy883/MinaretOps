namespace Core.DTOs.Branch
{
    public class CreateBranchDTO
    {
        public required string Name { get; set; }
        public string? Code { get; set; }
        public int CurrencyId { get; set; }
    }
}
