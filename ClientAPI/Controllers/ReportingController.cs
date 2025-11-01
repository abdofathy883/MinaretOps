using Core.Interfaces;
using Infrastructure.Services.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly IReportingService reportingService;

        public ReportingController(IReportingService reportingService)
        {
            this.reportingService = reportingService;
        }

        [HttpGet("task-employee-report")]
        public async Task<IActionResult> GetTaskEmployeeReport([FromQuery] string? currentUserId = null)
        {
            try
            {
                // If no currentUserId provided, try to get from claims
                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = User.FindFirst("sub")?.Value;
                }

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest("User ID is required");
                }

                var result = await reportingService.GetTaskEmployeeReportAsync(currentUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
