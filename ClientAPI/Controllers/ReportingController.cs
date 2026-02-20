using Core.Enums.Auth_Attendance;
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
        public async Task<IActionResult> GetTaskEmployeeReport(
            [FromQuery] string? currentUserId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
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

                // Validate date range if both dates are provided
                if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
                {
                    return BadRequest("FromDate cannot be greater than ToDate");
                }

                var result = await reportingService.GetTaskEmployeeReportAsync(currentUserId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("monthly-attendance-report")]
        public async Task<IActionResult> GetMonthlyAttendanceReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] AttendanceStatus? status = null)
        {
            try
            {
                if (fromDate == default(DateTime) || toDate == default(DateTime))
                {
                    return BadRequest("Both fromDate and toDate parameters are required (format: yyyy-MM-dd)");
                }

                var result = await reportingService.GetMonthlyAttendanceReportAsync(fromDate, toDate, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
