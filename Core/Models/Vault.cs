using Core.Enums;

namespace Core.Models
{
    public class Vault
    {
        public int Id { get; set; }
        public VaultType VaultType { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = default!;
        public List<VaultTransaction> Transactions { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
