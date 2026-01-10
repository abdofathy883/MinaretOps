using AutoMapper;
using Core.DTOs.Currency;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;

        public CurrencyService(MinaretOpsDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CurrencyDTO> CreateCurrencyAsync(CreateCurrencyDTO createCurrencyDTO)
        {
            var existingCurrencies = await context.Currencies
                .AnyAsync(c => c.Code == createCurrencyDTO.Code);

            if (existingCurrencies)
                throw new AlreadyExistObjectException("Currency is already there.");

            var currency = new Core.Models.Currency
            {
                Name = createCurrencyDTO.Name,
                Code = createCurrencyDTO.Code,
                DecimalPlaces = createCurrencyDTO.DecimalPlaces
            };

            await context.Currencies.AddAsync(currency);
            await context.SaveChangesAsync();
            return mapper.Map<CurrencyDTO>(currency);
        }

        public async Task<bool> DeleteCurrency(int id)
        {
            var currency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException("Couldn't find currency");

            context.Currencies.Remove(currency);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<CurrencyDTO>> GetAllCurrenciesAsync()
        {
            var currencies = await context.Currencies
                .ToListAsync();
            return mapper.Map<List<CurrencyDTO>>(currencies);
        }

        public async Task<CurrencyDTO> GetCurrencyByIdAsync(int id)
        {
            var currency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException("Couldn't find currency");

            return mapper.Map<CurrencyDTO>(currency);
        }

        public async Task<ExchangeRateDTO> AddExtchangeRate(CreateExchangeRateDTO createExchangeRateDTO)
        {
            // Validate currencies exist and are different
            if (createExchangeRateDTO.FromCurrencyId == createExchangeRateDTO.ToCurrencyId)
                throw new ArgumentException("FromCurrencyId and ToCurrencyId cannot be the same.");

            var fromCurrency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == createExchangeRateDTO.FromCurrencyId)
                ?? throw new KeyNotFoundException("FromCurrency not found.");

            var toCurrency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == createExchangeRateDTO.ToCurrencyId)
                ?? throw new KeyNotFoundException("ToCurrency not found.");

            // Deactivate existing active rates for this currency pair if new rate is active
            if (createExchangeRateDTO.IsActive)
            {
                var existingActiveRates = await context.ExchangeRates
                    .Where(er => er.FromCurrencyId == createExchangeRateDTO.FromCurrencyId
                        && er.ToCurrencyId == createExchangeRateDTO.ToCurrencyId
                        && er.IsActive)
                    .ToListAsync();

                foreach (var rate in existingActiveRates)
                {
                    rate.IsActive = false;
                    if (!rate.EffectiveTo.HasValue || rate.EffectiveTo.Value > createExchangeRateDTO.EffectiveFrom)
                    {
                        rate.EffectiveTo = createExchangeRateDTO.EffectiveFrom.AddDays(-1);
                    }
                }
            }

            var exchangeRate = new ExchangeRate
            {
                FromCurrencyId = createExchangeRateDTO.FromCurrencyId,
                ToCurrencyId = createExchangeRateDTO.ToCurrencyId,
                Rate = createExchangeRateDTO.Rate,
                EffectiveFrom = createExchangeRateDTO.EffectiveFrom,
                EffectiveTo = createExchangeRateDTO.EffectiveTo,
                IsActive = createExchangeRateDTO.IsActive
            };

            await context.ExchangeRates.AddAsync(exchangeRate);
            await context.SaveChangesAsync();
            return mapper.Map<ExchangeRateDTO>(exchangeRate);
        }

        public async Task<List<ExchangeRateDTO>> GetRatesByCurrencyId(int currencyId)
        {
            var rates = await context.ExchangeRates
                .Include(er => er.FromCurrency)
                .Include(er => er.ToCurrency)
                .Where(er => er.FromCurrencyId == currencyId || er.ToCurrencyId == currencyId)
                .OrderByDescending(er => er.EffectiveFrom)
                .ThenByDescending(er => er.IsActive)
                .ToListAsync();

            return mapper.Map<List<ExchangeRateDTO>>(rates);
        }
    }
}
