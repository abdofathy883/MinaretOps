using Core.DTOs.Tasks;
using Core.Settings;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

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
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
            });
        }

        public async Task StartAsync()
        {
            await client.LoginAsync(TokenType.Bot, options.Value.BotToken);
            await client.StartAsync();

            // Keep bot alive
            await Task.Delay(-1);
        }

        public async Task SendTaskNotification(string clientName, TaskDTO task)
        {
            foreach (var guild in client.Guilds)
            {
                foreach (var channel in guild.TextChannels)
                {
                    if (channel.Name.Equals(clientName, StringComparison.OrdinalIgnoreCase))
                    {
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
                        return;
                    }
                }
            }
            Console.WriteLine($"Channel '{clientName}' not found.");
        }

        //public async Task SendTaskToDiscord(TaskDTO task)
        //{
        //    var payload = new
        //    {
        //        embeds = new[]
        //        {
        //            new
        //            {
        //                title = $"New Task: {task.Title}",
        //                description = task.Description,
        //                color = 5814783, // Blue color
        //                fields = new[]
        //                {
        //                    new { name = "Assigned To", value = task.EmployeeName ?? "Unknown", inline = true },
        //                    new { name = "Due Date", value = task.Deadline.ToString("yyyy-MM-dd"), inline = true },
        //                    new { name = "Priority", value = task.Priority ?? "Normal", inline = true },
        //                    new { name = "Status", value = task.Status.ToString(), inline = true }
        //                },
        //                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
        //                footer = new { text = "The Minaret Agency Task Management" }
        //            }
        //        }
        //    };

        //    var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //    });

        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    await httpClient.PostAsync(discordWebhookUrl, content);
        //}
    }
}
