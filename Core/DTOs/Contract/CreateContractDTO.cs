namespace Core.DTOs.Contract
{
    public class CreateContractDTO
    {
        public int ClientId { get; set; }
        public int CurrencyId { get; set; }
        public int VaultId { get; set; }
        public int ContractDuration { get; set; }
        public decimal ContractTotal { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
