using Core.DTOs.VaultTransaction;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Finance")]
    public class VaultController : ControllerBase
    {
        private readonly IVaultService vaultService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public VaultController(IVaultService vaultService, IHttpContextAccessor httpContextAccessor)
        {
            this.vaultService = vaultService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await vaultService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("local")]
        public async Task<IActionResult> GetAllLocalAsync()
        {
            try
            {
                var result = await vaultService.GetAllLocalAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var result = await vaultService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("unified/{currencyId}")]
        public async Task<IActionResult> GetUnifiedVaultAsync(int currencyId)
        {
            try
            {
                var result = await vaultService.GetUnifiedVaultAsync(currencyId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetVaultBalanceAsync(int id, [FromQuery] int? currencyId = null)
        {
            try
            {
                var result = await vaultService.GetVaultBalanceAsync(id, currencyId);
                return Ok(new { vaultId = id, currencyId = currencyId, balance = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("unified/{currencyId}/balance")]
        public async Task<IActionResult> GetUnifiedVaultBalanceAsync(int currencyId)
        {
            try
            {
                var result = await vaultService.GetUnifiedVaultBalanceAsync(currencyId);
                return Ok(new { currencyId = currencyId, balance = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> GetVaultTransactionsAsync(int id, [FromQuery] VaultTransactionFilterDTO? filter = null)
        {
            try
            {
                var result = await vaultService.GetVaultTransactionsAsync(id, filter);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("unified/{currencyId}/transactions")]
        public async Task<IActionResult> GetUnifiedVaultTransactionsAsync(int currencyId, [FromQuery] VaultTransactionFilterDTO? filter = null)
        {
            try
            {
                var result = await vaultService.GetUnifiedVaultTransactionsAsync(currencyId, filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transactions")]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateVaultTransactionDTO createTransactionDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var result = await vaultService.CreateTransactionAsync(createTransactionDTO, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("transactions/{id}")]
        public async Task<IActionResult> UpdateTransactionAsync(int id, [FromBody] UpdateVaultTransactionDTO updateTransactionDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await vaultService.UpdateTransactionAsync(id, updateTransactionDTO);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("transactions/{id}")]
        public async Task<IActionResult> DeleteTransactionAsync(int id)
        {
            try
            {
                var result = await vaultService.DeleteTransactionAsync(id);
                if (result)
                    return Ok(new { message = "Transaction deleted successfully" });
                return BadRequest("Failed to delete transaction");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
