using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.OutboxProcessor
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(20);
        public OutboxProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
                        await handler.HandleAsync(msg, stoppingToken);
                        msg.ProcessedAt = DateTime.UtcNow;
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
