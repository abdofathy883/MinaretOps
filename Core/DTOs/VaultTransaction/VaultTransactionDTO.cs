using Core.Enums;

namespace Core.DTOs.VaultTransaction
{
    public class VaultTransactionDTO
    {
        public int Id { get; set; }
        public int VaultId { get; set; }
        public string? VaultBranchName { get; set; }
        public VaultType VaultType { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public required string CurrencyName { get; set; }
        public required string CurrencyCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public TransactionReferenceType ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? Notes { get; set; }
        public required string CreatedById { get; set; }
        public required string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
