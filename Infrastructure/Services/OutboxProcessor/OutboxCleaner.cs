using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.OutboxProcessor
{
    public class OutboxCleaner : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<OutboxCleaner> logger;
        private readonly TimeSpan interival = TimeSpan.FromDays(1);
        public OutboxCleaner(IServiceProvider _serviceProvider, ILogger<OutboxCleaner> _logger)
        {
            serviceProvider = _serviceProvider;
            logger = _logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<MinaretOpsDbContext>();
                    var readyToClean = await context.OutboxMessages
                        .Where(o => o.ProcessedAt != null)
                        .ToListAsync();
                    context.RemoveRange(readyToClean);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, "Error in Outbox Clean Job");
                }
                await Task.Delay(interival, stoppingToken);
            }
        }
    }
}
