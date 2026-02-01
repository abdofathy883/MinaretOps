using Core.DTOs.Contract;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Finance")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService contractService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ContractController(IContractService contractService, IHttpContextAccessor _httpContextAccessor)
        {
            this.contractService = contractService;
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await contractService.GetAll();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContractDTO createContract)
        {
            var user = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await contractService.Create(createContract, user);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await contractService.GetById(id);
            return Ok(result);
        }
    }
}
