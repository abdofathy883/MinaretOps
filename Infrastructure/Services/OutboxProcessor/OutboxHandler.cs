using Core.DTOs.Payloads;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Services.Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services.OutboxProcessor
{
    public class OutboxHandler : IOutboxHandler
    {
        private readonly IServiceProvider serviceProvider;
        public OutboxHandler(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
        }
        public async Task HandleAsync(Outbox message, CancellationToken token)
        {
            using var scope = serviceProvider.CreateScope();
            switch (message.OpType)
            {
                case OutboxTypes.Email:
                    var emailData = JsonSerializer.Deserialize<EmailPayload>(message.PayLoad);
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    await emailService.SendEmailWithTemplateAsync(
                        emailData.To, emailData.Subject, emailData.Template, emailData.Replacements
                    );
                    break;

                case OutboxTypes.Discord:
                    var discordData = JsonSerializer.Deserialize<DiscordPayload>(message.PayLoad);
                    var discordService = scope.ServiceProvider.GetRequiredService<DiscordService>();
                    switch (discordData.OperationType)
                    {
                        case DiscordOperationType.NewTask:
                            await discordService.NewTask(discordData.ChannelId, discordData.Task);
                            break;
                        case DiscordOperationType.UpdateTask:
                            await discordService.UpdateTask(discordData.ChannelId, discordData.Task);
                            break;
                        case DiscordOperationType.DeleteTask:
                            await discordService.DeleteTask(discordData.ChannelId, discordData.Task);
                            break;
                        case DiscordOperationType.CompleteTask:
                            await discordService.CompleteTask(discordData.ChannelId, discordData.Task);
                            break;
                        case DiscordOperationType.ChangeTaskStatus:
                            if (!discordData.NewStatus.HasValue)
                                throw new InvalidOperationException("NewStatus is required for ChangeTaskStatus operation");
                            await discordService.ChangeTaskStatus(discordData.ChannelId, discordData.Task, discordData.NewStatus.Value);
                            break;
                        case DiscordOperationType.NewComment:
                            await discordService.NewComment(discordData.ChannelId, discordData.Task);
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown Discord operation type: {discordData.OperationType}");
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown outbox type: {message.OpType}");
            }
        }
    }
}
