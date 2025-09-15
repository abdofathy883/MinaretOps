using Core.DTOs.Attendance;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService leaveRequestService;
        public LeaveRequestController(ILeaveRequestService service)
        {
            leaveRequestService = service;
        }
        [HttpPost("submit-leave-request")]
        public async Task<IActionResult> SubmitLeaveRequestAsync(CreateLeaveRequestDTO leaveRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newRequest = await leaveRequestService.SubmitLeaveRequest(leaveRequestDTO);
                return Ok(newRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("employee-leave-requests/{employeeId}")]
        public async Task<IActionResult> GetLeaveRequestsByEmployeeAsync(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return BadRequest("Employee ID is required.");
            }
            try
            {
                var requests = await leaveRequestService.GetLeaveRequestsByEmployee(employeeId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("all-leave-requests")]
        public async Task<IActionResult> GetAllLeaveRequestsAsync()
        {
            try
            {
                var requests = await leaveRequestService.GetAllLeaveRequests();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("admin-change-leave-request/{adminId}/{requestId}")]
        public async Task<IActionResult> ChangeLeaveRequestStatusByAdminAsync(string adminId, int requestId, [FromBody] LeaveStatus newStatus)
        {
            if (string.IsNullOrWhiteSpace(adminId))
            {
                return BadRequest("Admin ID is required.");
            }
            if (!Enum.IsDefined(typeof(LeaveStatus), newStatus))
            {
                return BadRequest("Invalid leave status.");
            }
            try
            {
                var updatedRequest = await leaveRequestService.ChangeLeaveRequestStatusByAdminAsync(adminId, requestId, newStatus);
                return Ok(updatedRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
