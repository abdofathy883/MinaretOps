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
            if (dto is null)
                throw new InvalidObjectException("");

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
                url
            });

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
                catch (Exception ex)
                {
                    // handle expired subscription (remove it from DB)
                    context.PushSubscriptions.Remove(sub);
                }
            }
        }
    }
}
