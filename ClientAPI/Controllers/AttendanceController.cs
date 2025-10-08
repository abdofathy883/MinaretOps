using Core.DTOs.Attendance;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;
        public AttendanceController(IAttendanceService attendance)
        {
            attendanceService = attendance;
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

        [HttpGet("all-attendance")]
        public async Task<IActionResult> GetAllAttendanceRecordsAsync()
        {
            try
            {
                var records = await attendanceService.GetAllAttendanceRecords();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

    }
}
