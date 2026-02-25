using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("CustomContract", Schema = "Clients")]
    public class CustomContract
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = default!;
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = default!;
        public int ContractDuration { get; set; }
        public decimal ContractTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount =>
            ContractTotal - PaidAmount;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
