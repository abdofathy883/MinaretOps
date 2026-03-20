namespace Application.DTOs.VaultTransaction
{
    public class UpdateVaultTransactionDTO
    {
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
    }
}
