using Core.DTOs.Tasks;
using Core.Enums;
using Core.Models;
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
        //public bool IsAvailable { get; private set; }
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
            //if (string.IsNullOrEmpty(options.Value.BotToken))
            //{
            //    IsAvailable = false;
            //    return;
            //}
            client.Ready += () =>
            {
                //IsAvailable = client.Guilds.Any();
                return Task.CompletedTask;
            };

            await client.LoginAsync(TokenType.Bot, options.Value.BotToken);
            await client.StartAsync();
        }

        public async Task<string> CreateChannelForClient(string clientName)
        {
            if (options.Value.GuildId == null)
                throw new InvalidOperationException("Discord GuildId is not configured");

            var guild = client.GetGuild(options.Value.GuildId.Value);
            if (guild == null)
                throw new InvalidObjectException("لم يتم العثور على سيرفر ديسكورد");

            // Sanitize client name for Discord channel name (Discord channel names have restrictions)
            var channelName = SanitizeChannelName(clientName);

            // Create a new text channel
            var channel = await guild.CreateTextChannelAsync(channelName, properties =>
            {
                properties.Topic = $"قناة إشعارات لعميل: {clientName}";
            });

            return channel.Id.ToString();
        }

        private string SanitizeChannelName(string name)
        {
            // Discord channel names must be lowercase, 1-100 characters, and can only contain alphanumeric characters, dashes, and underscores
            var sanitized = name.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("_", "-");

            // Remove any invalid characters
            sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"[^a-z0-9\-]", "");

            // Ensure it's not empty and within length limits
            if (string.IsNullOrWhiteSpace(sanitized))
                sanitized = "client-channel";

            if (sanitized.Length > 100)
                sanitized = sanitized.Substring(0, 100);

            return sanitized;
        }

        public async Task NewTask(string channelId, TaskItem task)
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

            embed.AddField("Assigned To", $"{task.Employee.FirstName} {task.Employee.LastName}" ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);
            embed.AddField("Due Date", task.Deadline.ToString("yyyy-MM-dd"), inline: true);
            embed.AddField("Priority", task.Priority ?? "عادي", inline: true);
            embed.AddField("Status", task.Status.ToString(), inline: true);
            embed.AddField("Task Type", task.TaskType.GetDescription(), inline: true);
            embed.AddField("Reference", task.Refrence ?? "N/A", inline: true);
            embed.AddField("Client", $"{task.ClientService.Client.Name}", inline: true);
            embed.AddField("Task Link", $"https://internal.theminaretagency.com/tasks/{task.Id}");

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task UpdateTask(string channelId, TaskItem task)
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

            embed.AddField("Assigned To", $"{task.Employee.FirstName} {task.Employee.LastName}" ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);
            embed.AddField("Due Date", task.Deadline.ToString("yyyy-MM-dd"), inline: true);
            embed.AddField("Priority", task.Priority ?? "عادي", inline: true);
            embed.AddField("Status", task.Status.ToString(), inline: true);
            embed.AddField("Task Type", task.TaskType.GetDescription(), inline: true);
            embed.AddField("Reference", task.Refrence ?? "N/A", inline: true);
            embed.AddField("Client", $"{task.ClientService.Client.Name}", inline: true);
            embed.AddField("Task Link", $"https://internal.theminaretagency.com/tasks/{task.Id}");

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task DeleteTask(string channelId, TaskItem task)
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

            embed.AddField("Assigned To", $"{task.Employee.FirstName} {task.Employee.LastName}" ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task CompleteTask(string channelId, TaskItem task)
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

            embed.AddField("Assigned To", $"{task.Employee.FirstName} {task.Employee.LastName}" ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task NewComment(string channelId, TaskItem task)
        {
            var parsedChannel = ulong.Parse(channelId);
            var channel = client.GetChannel(parsedChannel) as IMessageChannel;
            if (channel is null)
                throw new InvalidObjectException("لم يتم العثور على قناة ديسكورد لهذا المعرف");

            var embed = new EmbedBuilder
            {
                Title = $"New Comment On Task: {task.Title}",
                Color = new Color(145, 240, 11), // RGB for lime
                Timestamp = DateTimeOffset.UtcNow
            };

            embed.AddField("Assigned To", $"{task.Employee.FirstName} {task.Employee.LastName}" ?? "Unknown", inline: true);
            embed.AddField("Task Id", task.Id, inline: true);
            embed.AddField("Status", task.Status.ToString(), inline: true);
            embed.AddField("Reference", task.Refrence ?? "N/A", inline: true);

            // Get last comment (defensive: null / empty checks). Prefer latest by Id if no created timestamp.
            var lastComment = task.TaskComments?.OrderByDescending(c => c.Id).FirstOrDefault();

            var author = !string.IsNullOrWhiteSpace(lastComment.Employee?.FirstName) || !string.IsNullOrWhiteSpace(lastComment.Employee?.LastName)
                ? $"{lastComment.Employee?.FirstName} {lastComment.Employee?.LastName}".Trim()
                : lastComment.EmployeeId ?? "Unknown";

            // Limit length to avoid overly large embed fields (Discord max ~1024 chars per field)
            var trimmedComment = lastComment.Comment?.Trim() ?? string.Empty;
            if (trimmedComment.Length > 900)
                trimmedComment = trimmedComment[..900] + "…";

            var commentText = $"{trimmedComment}\n— {author}";

            embed.AddField("Comment", commentText);
            embed.AddField("Added By", author);
            embed.AddField("Task Link", $"https://internal.theminaretagency.com/tasks/{task.Id}");

            embed.WithFooter("The Minaret Agency Task Management");

            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task ChangeTaskStatus(string channelId, TaskItem task, CustomTaskStatus status)
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

            embed.AddField("Assigned To", $"{task.Employee.FirstName} {task.Employee.LastName}" ?? "Unknown", inline: true);
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
