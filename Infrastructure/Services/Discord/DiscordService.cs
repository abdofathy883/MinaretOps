using Core.DTOs.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services.Discord
{
    public class DiscordService
    {
        private readonly HttpClient httpClient;
        private readonly string discordWebhookUrl = "https://discord.com/api/webhooks/1414113831101141042/pQs1V4ULheEPb38jPXnZBhxD5qFDkLrqm8CL6xUNGReaUd4jJXbOje4AFP0aHQpHzj5A";
        public DiscordService(HttpClient http)
        {
            httpClient = http;
        }

        public async Task SendTaskToDiscord(TaskDTO task)
        {
            var payload = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = $"New Task: {task.Title}",
                        description = task.Description,
                        color = 5814783, // Blue color
                        fields = new[]
                        {
                            new { name = "Assigned To", value = task.EmployeeName ?? "Unknown", inline = true },
                            new { name = "Due Date", value = task.Deadline.ToString("yyyy-MM-dd"), inline = true },
                            new { name = "Priority", value = task.Priority ?? "Normal", inline = true },
                            new { name = "Status", value = task.Status.ToString(), inline = true }
                        },
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        footer = new { text = "The Minaret Agency Task Management" }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await httpClient.PostAsync(discordWebhookUrl, content);
        }
    }
}
