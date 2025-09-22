using Core.DTOs.Tasks;
using Core.Settings;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Infrastructure.Services.Discord
{
    public class DiscordService
    {
        private readonly DiscordSocketClient client;
        private readonly IOptions<DiscordSettings> options;
        public DiscordService(IOptions<DiscordSettings> options)
        {
            this.options = options;
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged
                //GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
            });
        }

        public async Task StartAsync()
        {
            client.Ready += () =>
            {
                Console.WriteLine("Bot is connected and ready!");
                return Task.CompletedTask;
            };

            await client.LoginAsync(TokenType.Bot, options.Value.BotToken);
            Console.WriteLine("token: ", options.Value.BotToken);
            await client.StartAsync();

            //// Keep bot alive
            //await Task.Delay(-1);
        }

        public async Task SendTaskNotification(string channelId, TaskDTO task)
        {
            var parsedChannel = ulong.Parse(channelId);
            var channel = client.GetChannel(parsedChannel) as IMessageChannel;
            if (channel is null)
                throw new Exception();

            var embed = new EmbedBuilder
            {
                Title = $"New Task: {task.Title}",
                Description = task.Description,
                Color = new Color(89, 125, 245), // RGB for blue
                Timestamp = DateTimeOffset.UtcNow
            };

            embed.AddField("Assigned To", task.EmployeeName ?? "Unknown", inline: true);
            embed.AddField("Due Date", task.Deadline.ToString("yyyy-MM-dd"), inline: true);
            embed.AddField("Priority", task.Priority ?? "عادي", inline: true);
            embed.AddField("Status", task.Status.ToString(), inline: true);
            embed.AddField("Task Type", task.TaskType.ToString(), inline: true);
            embed.AddField("Reference", task.Refrence ?? "N/A", inline: true);
            embed.AddField("Client", task.ClientName, inline: true);

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
