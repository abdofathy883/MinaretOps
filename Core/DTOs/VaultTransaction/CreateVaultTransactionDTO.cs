using Core.Enums;

namespace Core.DTOs.VaultTransaction
{
    public class CreateVaultTransactionDTO
    {
        public int VaultId { get; set; }
        public int CurrencyId { get; set; }
        public required string UserId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public TransactionReferenceType ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? Notes { get; set; }
    }
}
