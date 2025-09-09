using Core.DTOs.KPI;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIController : ControllerBase
    {
        private readonly IKPIService kPIService;
        public KPIController(IKPIService service)
        {
            kPIService = service;
        }

        [HttpPost] 
        public async Task<IActionResult> NewKPIIncedint(CreateIncedintDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var result = await kPIService.NewKPIIncedintAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
