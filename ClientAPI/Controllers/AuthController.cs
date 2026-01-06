using Core.DTOs.AuthDTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService _authService)
        {
            authService = _authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO request)
        {
            var result = await authService.LoginAsync(request);
            if (result.IsAuthenticated)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserDTO request)
        {
            if (request is null)
                return BadRequest("بيانات المستخدم غير صحيحة");
            try
            {
                var result = await authService.RegisterUserAsync(request);
                if (result.IsAuthenticated)
                    return Ok(result);
                return BadRequest();

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await authService.GetAllUsersAsync();
            if (users.Count > 0)
            {
                return Ok(users);
            }
            return NotFound("لا يوجد مستخدمين");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            var user = await authService.GetUserByIdAsync(userId);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("المستخدم غير موجود");
        }

        [HttpPatch("update-user")]
        public async Task<IActionResult> UpdateAsync(UpdateUserDTO updateUserDTO)
        {
            if (updateUserDTO is null)
                return BadRequest();

            var result = await authService.UpdateUserAsync(updateUserDTO);
            if (result.IsAuthenticated)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPatch("set-password")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO is null)
                return BadRequest();

            var result = await authService.ChangePasswordAsync(changePasswordDTO);
            if (!result.IsAuthenticated)
                return BadRequest();

            return Ok(result);
        }
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var result = await authService.DeleteUserAsync(userId);

            return Ok(result);
        }

        [HttpPost("send-reset-link/{userId}")]
        public async Task<IActionResult> SendResetLinkAsync(string userId)
        {
            var link = await authService.RequestResetPasswordByAdminAsync(userId);
            return Ok(new { link });

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            await authService.ResetPAsswordAsync(resetPasswordDTO);
            return Ok();
        }
    }
}
