using Core.DTOs.Notifications;
using Core.Interfaces;
using Infrastructure.Services.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;
        public NotificationController(INotificationService service) 
            => notificationService = service;

        [HttpGet("today/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetToday(string userId)
        {
            var notifications = await notificationService.GetTodayForUserAsync(userId);
            return Ok(notifications);
        }

        [HttpPatch("read/{id}/{userId}")]
        public async Task<IActionResult> MarkAsRead(int id, string userId)
        {
            await notificationService.MarkAsReadAsync(id, userId);
            return NoContent();
        }
    }
}
