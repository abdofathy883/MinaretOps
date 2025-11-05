using Core.DTOs.EmployeeOnBoarding;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeInvitationController : ControllerBase
    {
        private readonly IInvitationService invitationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public EmployeeInvitationController(
            IInvitationService _invitationService,
            IHttpContextAccessor _httpContextAccessor)
        {
            invitationService = _invitationService;
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateInvitationAsync([FromBody] CreateInvitationDTO dto)
        {
            var adminUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminUserId))
                return Unauthorized();

            var invitation = await invitationService.CreateInvitationAsync(dto, adminUserId);
            return Ok(invitation);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingInvitationsAsync()
        {
            var invitations = await invitationService.GetPendingInvitationsAsync();
            return Ok(invitations);
        }

        [HttpGet("token/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInvitationByTokenAsync(string token)
        {
            var invitation = await invitationService.GetInvitationByTokenAsync(token);
            return Ok(invitation);
        }

        [HttpPost("complete")]
        [AllowAnonymous]
        public async Task<IActionResult> CompleteInvitationAsync([FromBody] CompleteInvitationDTO dto)
        {
            var invitation = await invitationService.CompleteInvitationAsync(dto);
            return Ok(invitation);
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveInvitationAsync(int id)
        {
            var adminUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminUserId))
                return Unauthorized();

            await invitationService.ApproveInvitationAsync(id, adminUserId);
            return Ok(new { message = "تم الموافقة على الدعوة وإنشاء الحساب بنجاح" });
        }

        [HttpPost("cancel/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelInvitationAsync(int id)
        {
            await invitationService.CancelInvitationAsync(id);
            return Ok(new { message = "تم إلغاء الدعوة" });
        }
    }
}