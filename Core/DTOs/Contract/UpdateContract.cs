using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Contract
{
    public class UpdateContract
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public int ContractDuration { get; set; }
        public decimal ContractTotal { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
