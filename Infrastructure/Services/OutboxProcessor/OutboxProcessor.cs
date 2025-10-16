using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.OutboxProcessor
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(20);
        private readonly ILogger<OutboxProcessor> _logger;
        public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<MinaretOpsDbContext>();
                    var handler = scope.ServiceProvider.GetRequiredService<IOutboxHandler>();
                    var discordService = scope.ServiceProvider.GetRequiredService<DiscordService>();
                    var pending = await context.OutboxMessages
                        .Where(m => m.ProcessedAt == null)
                        .OrderBy(m => m.CreatedAt)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var msg in pending)
                    {
                        try
                        {
                            await handler.HandleAsync(msg, stoppingToken);
                            msg.ProcessedAt = DateTime.UtcNow;
                        }
                        catch (Exception msgEx)
                        {
                            // Log and mark as processed to avoid poison-message infinite loops
                            _logger.LogError(msgEx, "Outbox message processing failed. Id={OutboxId} Title={OutboxTitle}", msg.Id, msg.OpTitle);
                            msg.ProcessedAt = DateTime.UtcNow;
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log and continue without crashing the host
                    _logger.LogError(ex, "OutboxProcessor cycle failed");
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
