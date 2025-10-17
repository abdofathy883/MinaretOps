using Core.DTOs.Notifications;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushSubscriptionController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public PushSubscriptionController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionDTO subscription)
        {
            try
            {
                await notificationService.SubscribeUserAsync(subscription);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("unsubscribe/{userId}")]
        public async Task<IActionResult> Unsubscribe(string userId)
        {
            try
            {
                await notificationService.UnsubscribeUserAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
