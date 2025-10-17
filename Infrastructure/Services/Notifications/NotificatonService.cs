using AutoMapper;
using Core.DTOs.Notifications;
using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services.Notifications
{
    public class NotificatonService : INotificationService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;
        private readonly IOptions<VapidDetails> vapidDetails;
        public NotificatonService(
            MinaretOpsDbContext minaret,
            IMapper _mapper,
            IOptions<VapidDetails> _vapidDetails
            )
        {
            context = minaret;
            mapper = _mapper;
            vapidDetails = _vapidDetails;
        }
        public async Task<NotificationDTO> CreateAsync(CreateNotificationDTO dto)
        {
            var notification = new PushNotification
            {
                Title = dto.Title,
                Body = dto.Body,
                UserId = dto.UserId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Url = dto.Url
            };

            await context.PushNotifications.AddAsync(notification);
            await context.SaveChangesAsync();

            await SendNotificationAsync(dto.UserId, dto.Title, dto.Body, dto.Url);
            return mapper.Map<NotificationDTO>(notification);
        }

        public async Task<IEnumerable<NotificationDTO>> GetTodayForUserAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;

            var notifications = await context.PushNotifications.
                Where(n => n.UserId == userId && n.CreatedAt.Date == today)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<NotificationDTO>>(notifications);
        }

        public async Task MarkAsReadAsync(int id, string userId)
        {
            var notif = await context.PushNotifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notif is null)
                throw new KeyNotFoundException("Notification not found");

            notif.IsRead = true;
            await context.SaveChangesAsync();
        }

        public async Task SendNotificationAsync(string userId, string title, string body, string url)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var subscriptions = await context.PushSubscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (!subscriptions.Any()) return;

            var pushClient = new PushServiceClient();
            pushClient.DefaultAuthentication = new VapidAuthentication(vapidDetails.Value.PublicKey, vapidDetails.Value.PrivateKey);

            var payload = JsonSerializer.Serialize(new
            {
                title,
                body,
                url,
                icon = "assets/icons/icon-192x192.png",
                badge = "assets/icons/icon-72x72.jpg",
                sound = "default",
                vibrate = new int[] { 200, 100, 200 },
                requireInteraction = true,
                tag = "announcement",
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });

            var expiredSubscriptions = new List<CustomPushSubscription>();

            foreach (var sub in subscriptions)
            {
                var pushSub = new PushSubscription
                {
                    Endpoint = sub.Endpoint,
                    Keys = new Dictionary<string, string>
                    {
                        ["p256dh"] = sub.P256DH,
                        ["auth"] = sub.Auth
                    }
                };

                try
                {
                    await pushClient.RequestPushMessageDeliveryAsync(pushSub, new PushMessage(payload));
                }
                catch (Exception)
                {
                    expiredSubscriptions.Add(sub);
                }
            }
            // Remove expired subscriptions
            if (expiredSubscriptions.Any())
            {
                context.PushSubscriptions.RemoveRange(expiredSubscriptions);
                await context.SaveChangesAsync();
            }
        }

        public async Task SubscribeUserAsync(PushSubscriptionDTO subscription)
        {
            if (subscription == null)
                throw new InvalidObjectException("Subscription data cannot be null");

            if (string.IsNullOrEmpty(subscription.UserId) || string.IsNullOrEmpty(subscription.Endpoint))
                throw new InvalidObjectException("UserId and Endpoint are required");

            // Check if subscription already exists for this user and endpoint
            var existingSubscription = await context.PushSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == subscription.UserId && s.Endpoint == subscription.Endpoint);

            if (existingSubscription != null)
            {
                // Update existing subscription
                existingSubscription.P256DH = subscription.Keys.GetProperty("p256dh").GetString();
                existingSubscription.Auth = subscription.Keys.GetProperty("auth").GetString();
            }
            else
            {
                // Create new subscription
                var newSubscription = new CustomPushSubscription
                {
                    UserId = subscription.UserId,
                    Endpoint = subscription.Endpoint,
                    P256DH = subscription.Keys.GetProperty("p256dh").GetString(),
                    Auth = subscription.Keys.GetProperty("auth").GetString()
                };

                await context.PushSubscriptions.AddAsync(newSubscription);
            }

            await context.SaveChangesAsync();
        }

        public async Task UnsubscribeUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new InvalidObjectException("UserId is required");

            var subscriptions = await context.PushSubscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (subscriptions.Any())
            {
                context.PushSubscriptions.RemoveRange(subscriptions);
                await context.SaveChangesAsync();
            }
        }
    }
}
