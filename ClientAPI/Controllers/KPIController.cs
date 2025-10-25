using Core.DTOs.KPI;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
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
        [Consumes("multipart/form-data")]
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
        public async Task<IActionResult> GetEmployeeSummary(string employeeId, [FromQuery] int? month, [FromQuery] int? year)
        {
            var res = await kPIService.GetEmployeeMonthlyAsync(employeeId, month, year);
            return Ok(res);
        }

        [HttpGet("all-summaries")]
        public async Task<IActionResult> GetAllSummaries([FromQuery] int? month, [FromQuery] int? year)
        {
            var res = await kPIService.GetMonthlySummeriesAsync(month, year);
            return Ok(res);
        }

        [HttpGet("incidents/{employeeId}")]
        public async Task<IActionResult> GetIncidents(string employeeId, [FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            var res = await kPIService.GetIncedientsByEmpIdAsync(employeeId, month, year);
            return Ok(res);
        }

        [HttpGet("get-all-incedints")]
        public async Task<IActionResult> GetAllIncedintsAsync()
        {
            var result = await kPIService.GetAllIncedientsAsync();
            return Ok(result);
        }
    }
}
