using System.Text.Json;

namespace Core.DTOs.Notifications
{
    public class PushSubscriptionDTO
    {
        public string UserId { get; set; }
        public string Endpoint { get; set; }
        public JsonElement Keys { get; set; }
    }
}
