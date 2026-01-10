using Core.Enums;

namespace Core.DTOs.Contract
{
    public class ContractDTO
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public required string CurrencyName { get; set; }
        public int ClientId { get; set; }
        public required string ClientName { get; set; }
        public decimal ServiceCost { get; set; }
        public required string ServiceName { get; set; }
        public required string AccountManagerName { get; set; }
        public required string Country { get; set; }
        public BusinessType BusinessType { get; set; }
        public int ContractDuration { get; set; }
        public decimal ContractTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount =>
            ContractTotal - PaidAmount;
    }
}
