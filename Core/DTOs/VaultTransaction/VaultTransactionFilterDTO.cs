using Core.Enums;

namespace Core.DTOs.VaultTransaction
{
    public class VaultTransactionFilterDTO
    {
        public int? VaultId { get; set; }
        public TransactionType? TransactionType { get; set; }
        public TransactionReferenceType? ReferenceType { get; set; }
        public int? CurrencyId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ReferenceId { get; set; }
    }
}
