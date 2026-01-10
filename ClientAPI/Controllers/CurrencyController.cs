using Core.DTOs.Currency;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCurrency(CreateCurrencyDTO createCurrencyDTO)
        {
            if (createCurrencyDTO == null)
                return BadRequest();
            try
            {
                var result = await currencyService.CreateCurrencyAsync(createCurrencyDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCurrencies()
        {
            var result = await currencyService.GetAllCurrenciesAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrency(int id)
        {
            var result = await currencyService.GetCurrencyByIdAsync(id);
            return Ok(result);
        }
        [HttpPost("exchange-rate")]
        public async Task<IActionResult> CreateExchangeRate(CreateExchangeRateDTO createExchangeRateDTO)
        {
            if (createExchangeRateDTO == null)
                return BadRequest();
            try
            {
                var result = await currencyService.AddExtchangeRate(createExchangeRateDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/exchange-rates")]
        public async Task<IActionResult> GetExchangeRatesByCurrencyId(int id)
        {
            try
            {
                var result = await currencyService.GetRatesByCurrencyId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
