using Core.DTOs.Attendance;
using Core.DTOs.AttendanceBreaks;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.Services.Attendance;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("clock-out/{empId}")]
        public async Task<IActionResult> ClockOutAsync(string empId)
        {
            var result = await attendanceService.ClockOutAsync(empId);
            return Ok(result);
        }

        [HttpGet("today-attendance/{employeeId}")]
        public async Task<IActionResult> GetTodayAttendanceAsync(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return BadRequest();
            var attendances = await attendanceService.GetTodayAttendanceForEmployeeAsync(employeeId);
            return Ok(attendances);
        }

        [HttpGet("employee-attendance/{employeeId}")]
        public async Task<IActionResult> GetAttendanceRecordsByEmployeeAsync(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return BadRequest("Employee ID is required.");
            }
            try
            {
                var records = await attendanceService.GetTodayAttendanceForEmployeeAsync(employeeId);
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

        [HttpPut("admin-change-attendance/{adminId}/{recordId}")]
        public async Task<IActionResult> ChangeAttendanceStatusByAdminAsync(string adminId, int recordId, [FromBody] AttendanceStatus newStatus)
        {
            if (string.IsNullOrWhiteSpace(adminId))
            {
                return BadRequest("Admin ID is required.");
            }
            if (!Enum.IsDefined(typeof(AttendanceStatus), newStatus))
            {
                return BadRequest("Invalid attendance status.");
            }
            try
            {
                var updatedRecord = await attendanceService.ChangeAttendanceStatusByAdminAsync(adminId, recordId, newStatus);
                return Ok(updatedRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Add these methods to the existing AttendanceController

        [HttpPost("start-break")]
        public async Task<IActionResult> StartBreakAsync([FromBody] Start_EndBreakDTO breakDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await breakService.StartBreakAsync(breakDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("end-break")]
        public async Task<IActionResult> EndBreakAsync([FromBody] Start_EndBreakDTO breakDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await breakService.EndBreakAsync(breakDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("active-break/{employeeId}")]
        public async Task<IActionResult> GetActiveBreakAsync(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return BadRequest("Employee ID is required");

            try
            {
                var result = await breakService.GetActiveBreakAsync(employeeId);
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
