using Core.DTOs.Tasks;
using Core.Enums;
using Core.Settings;
using Discord;
using Discord.WebSocket;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Reflection;
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
                //GatewayIntents = GatewayIntents.AllUnprivileged
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
            });
        }

        public async Task StartAsync()
        {
            client.Ready += () =>
            {
                return Task.CompletedTask;
            };

            await client.LoginAsync(TokenType.Bot, options.Value.BotToken);
            await client.StartAsync();
        }
        public async Task NewTask(string channelId, TaskDTO task)
        {
            var parsedChannel = ulong.Parse(channelId);
            var channel = client.GetChannel(parsedChannel) as IMessageChannel;
            if (channel is null)
                throw new InvalidObjectException("لم يتم العثور على قناة ديسكورد لهذا المعرف");

            var embed = new EmbedBuilder
            {
                Title = $"New Task: {task.Title}",
                Description = task.Description,
                Color = new Color(145, 240, 11), // RGB for lime
                Timestamp = DateTimeOffset.UtcNow
            };

            embed.AddField("Assigned To", task.EmployeeName ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);
            embed.AddField("Due Date", task.Deadline.ToString("yyyy-MM-dd"), inline: true);
            embed.AddField("Priority", task.Priority ?? "عادي", inline: true);
            embed.AddField("Status", task.Status.ToString(), inline: true);
            embed.AddField("Task Type", task.TaskType.GetDescription(), inline: true);
            embed.AddField("Reference", task.Refrence ?? "N/A", inline: true);
            embed.AddField("Client", task.ClientName, inline: true);
            embed.AddField("Task Link", $"https://internal.theminaretagency.com/tasks/{task.Id}");

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task UpdateTask(string channelId, TaskDTO task)
        {
            var parsedChannel = ulong.Parse(channelId);
            var channel = client.GetChannel(parsedChannel) as IMessageChannel;
            if (channel is null)
                throw new InvalidObjectException("لم يتم العثور على قناة ديسكورد لهذا المعرف");

            var embed = new EmbedBuilder
            {
                Title = $"Task Update: {task.Title}",
                Description = task.Description,
                Color = new Color(145, 240, 11), // RGB for lime
                Timestamp = DateTimeOffset.UtcNow
            };

            embed.AddField("Assigned To", task.EmployeeName ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);
            embed.AddField("Due Date", task.Deadline.ToString("yyyy-MM-dd"), inline: true);
            embed.AddField("Priority", task.Priority ?? "عادي", inline: true);
            embed.AddField("Status", task.Status.ToString(), inline: true);
            embed.AddField("Task Type", task.TaskType.GetDescription(), inline: true);
            embed.AddField("Reference", task.Refrence ?? "N/A", inline: true);
            embed.AddField("Client", task.ClientName, inline: true);
            embed.AddField("Task Link", $"https://internal.theminaretagency.com/tasks/{task.Id}");

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task DeleteTask(string channelId, TaskDTO task)
        {
            var parsedChannel = ulong.Parse(channelId);
            var channel = client.GetChannel(parsedChannel) as IMessageChannel;
            if (channel is null)
                throw new InvalidObjectException("لم يتم العثور على قناة ديسكورد لهذا المعرف");

            var embed = new EmbedBuilder
            {
                Title = $"Task Deleted: {task.Title}",
                Color = new Color(145, 240, 11), // RGB for lime
                Timestamp = DateTimeOffset.UtcNow
            };

            embed.AddField("Assigned To", task.EmployeeName ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task CompleteTask(string channelId, TaskDTO task)
        {
            var parsedChannel = ulong.Parse(channelId);
            var channel = client.GetChannel(parsedChannel) as IMessageChannel;
            if (channel is null)
                throw new InvalidObjectException("لم يتم العثور على قناة ديسكورد لهذا المعرف");

            var embed = new EmbedBuilder
            {
                Title = $"Task Completed: {task.Title}",
                Color = new Color(145, 240, 11), // RGB for lime
                Timestamp = DateTimeOffset.UtcNow
            };

            embed.AddField("Assigned To", task.EmployeeName ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task ChangeTaskStatus(string channelId, TaskDTO task, CustomTaskStatus status)
        {
            var parsedChannel = ulong.Parse(channelId);
            var channel = client.GetChannel(parsedChannel) as IMessageChannel;
            if (channel is null)
                throw new InvalidObjectException("لم يتم العثور على قناة ديسكورد لهذا المعرف");

            var embed = new EmbedBuilder
            {
                Title = $"Task Update: {task.Title}",
                Description = task.Description,
                Color = new Color(145, 240, 11), // RGB for lime
                Timestamp = DateTimeOffset.UtcNow
            };

            embed.AddField("Assigned To", task.EmployeeName ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);
            embed.AddField("Old Status", task.Status.ToString(), inline: true);
            embed.AddField("New Status", status, inline: true);
            embed.AddField("Task Type", task.TaskType.GetDescription(), inline: true);
            embed.AddField("Task Link", $"https://internal.theminaretagency.com/tasks/{task.Id}");

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
    }

    public static class EnumToString
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = (DescriptionAttribute)field
                .GetCustomAttribute(typeof(DescriptionAttribute));

            return attr != null ? attr.Description : value.ToString();
        }
    }
}
