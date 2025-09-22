using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        {
            try
            {
            await discordService.StartAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("error////////////////////:", ex);
                throw new Exception(ex.Message);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken) 
            => Task.CompletedTask;
    }
}
