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

        [HttpGet("summary/{employeeId}")]
        public async Task<IActionResult> GetEmployeeSummary(string employeeId, [FromQuery] int year, [FromQuery] int month)
        {
            var res = await kPIService.GetEmployeeMonthlyAsync(employeeId, year, month);
            return Ok(res);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAllSummaries([FromQuery] int year, [FromQuery] int month)
        {
            var res = await kPIService.GetMonthlySummeriesAsync(year, month);
            return Ok(res);
        }

        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents([FromQuery] string? employeeId, [FromQuery] int year, [FromQuery] int month)
        {
            var res = await kPIService.GetIncedientsAsync(employeeId, year, month);
            return Ok(res);
        }
    }
}
