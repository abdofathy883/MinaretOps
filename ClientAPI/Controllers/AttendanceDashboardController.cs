using Core.Interfaces;
using Infrastructure.Services.Attendance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceDashboardController : ControllerBase
    {
        private readonly IAttendanceDashboard attendanceDashboard;

        public AttendanceDashboardController(IAttendanceDashboard attendanceDashboard)
        {
            this.attendanceDashboard = attendanceDashboard;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetAttendanceDashboard()
        {
            try
            {
                var dashboard = await attendanceDashboard.GetAttendanceDashboardAsync();
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
