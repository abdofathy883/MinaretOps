using Core.Enums;

namespace Core.DTOs.Vault
{
    public class VaultDTO
    {
        public int Id { get; set; }
        public VaultType VaultType { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int CurrencyId { get; set; }
        public required string CurrencyName { get; set; }
        public required string CurrencyCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Balance { get; set; }
    }
}
