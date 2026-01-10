using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }

        public int FromCurrencyId { get; set; }
        public Currency FromCurrency { get; set; } = default!;

        public int ToCurrencyId { get; set; }
        public Currency ToCurrency { get; set; } = default!;

        public decimal Rate { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
    }
}
