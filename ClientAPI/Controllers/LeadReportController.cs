using Application.Interfaces.Leads;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadReportController : ControllerBase
    {
        private readonly ILeadReportService _leadReportService;

        public LeadReportController(ILeadReportService leadReportService)
        {
            _leadReportService = leadReportService;
        }

        [HttpGet("get-lead-employee-report")]
        public async Task<IActionResult> GetLeadEmployeeReport(
            [FromQuery] string? currentUserId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = User.FindFirst("sub")?.Value;
                }

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest("User ID is required");
                }

                if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
                {
                    return BadRequest("FromDate cannot be greater than ToDate");
                }

                var result = await _leadReportService.GetLeadsEmployeeReportAsync(currentUserId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
