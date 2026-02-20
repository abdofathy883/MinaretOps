using Core.DTOs.Attendance;
using Core.Enums.Auth_Attendance;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;
        private readonly IBreakService breakService;
        public AttendanceController(IAttendanceService attendance, IBreakService breakService)
        {
            attendanceService = attendance;
            this.breakService = breakService;
        }

        [HttpPost("clock-in")]
        public async Task<IActionResult> CreateNewAttendanceRecordAsync(CreateAttendanceRecordDTO recordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newRecord = await attendanceService.ClockInAsync(recordDTO);
                return Ok(newRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("clock-out")]
        public async Task<IActionResult> ClockOutAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await attendanceService.ClockOutAsync(userId);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("today-attendance")]
        public async Task<IActionResult> GetTodayAttendanceAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var attendances = await attendanceService.GetTodayAttendanceForEmployeeAsync(userId);
                return Ok(attendances);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("employee-attendance")]
        public async Task<IActionResult> GetAttendanceRecordsByEmployeeAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var records = await attendanceService.GetTodayAttendanceForEmployeeAsync(userId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("all-attendance/{date}")]
        public async Task<IActionResult> GetAllAttendanceRecordsAsync(DateOnly date)
        {
            try
            {
                var records = await attendanceService.GetAllAttendanceRecords(date);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedAttendance(
            [FromQuery] string? fromDate,
            [FromQuery] string? toDate,
            [FromQuery] string? employeeId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var filter = new AttendanceFilterDTO
            {
                FromDate = string.IsNullOrEmpty(fromDate) ? null : DateOnly.Parse(fromDate),
                ToDate = string.IsNullOrEmpty(toDate) ? null : DateOnly.Parse(toDate),
                EmployeeId = employeeId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await attendanceService.GetAttendanceRecordsAsync(filter);
            return Ok(result);
        }

        [HttpPut("admin-change-attendance/{recordId}")]
        public async Task<IActionResult> ChangeAttendanceStatusByAdminAsync(int recordId, [FromBody] AttendanceStatus newStatus)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Enum.IsDefined(typeof(AttendanceStatus), newStatus))
            {
                return BadRequest("Invalid attendance status.");
            }
            try
            {
                var updatedRecord = await attendanceService.ChangeAttendanceStatusByAdminAsync(userId, recordId, newStatus);
                return Ok(updatedRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("start-break")]
        public async Task<IActionResult> StartBreakAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var result = await breakService.StartBreakAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("end-break")]
        public async Task<IActionResult> EndBreakAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var result = await breakService.EndBreakAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("active-break")]
        public async Task<IActionResult> GetActiveBreakAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var result = await breakService.GetActiveBreakAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("submit-early-leave")]
        public async Task<IActionResult> SubmitEarlyLeaveAsync(ToggleEarlyLeaveDTO earlyLeave)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await attendanceService.SubmitEarlyLeaveByEmpIdAsync(earlyLeave);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
