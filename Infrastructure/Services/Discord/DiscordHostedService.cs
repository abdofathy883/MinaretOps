using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.Discord
{
    public class DiscordHostedService : IHostedService
    {
        private readonly DiscordService discordService;
        public DiscordHostedService(DiscordService service)
        {
            discordService = service;
        }
        public async Task StartAsync(CancellationToken cancellationToken) 
            => await discordService.StartAsync();

        public Task StopAsync(CancellationToken cancellationToken) 
            => Task.CompletedTask;
    }
}
