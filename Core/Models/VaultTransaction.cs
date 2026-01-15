using Core.Enums;

namespace Core.Models
{
    public class VaultTransaction
    {
        public int Id { get; set; }
        public int VaultId { get; set; }
        public Vault Vault { get; set; } = default!;
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = default!;
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public TransactionReferenceType ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? Notes { get; set; }
        public required string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
