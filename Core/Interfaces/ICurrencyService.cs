using Core.DTOs.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICurrencyService
    {
        Task<CurrencyDTO> CreateCurrencyAsync(CreateCurrencyDTO createCurrencyDTO);
        Task<List<CurrencyDTO>> GetAllCurrenciesAsync();
        Task<CurrencyDTO> GetCurrencyByIdAsync(int id);
        Task<List<ExchangeRateDTO>> GetRatesByCurrencyId(int currencyId);
        Task<ExchangeRateDTO> AddExtchangeRate(CreateExchangeRateDTO createExchangeRateDTO);
        Task<bool> DeleteCurrency(int id);
    }
}
