using Core.DTOs.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
                Title = task.Title,
                Description = task.Description,
                Color = 5814783, // Example color in decimal,
                Fields = new[]
                {
                    new { name = "Assigned To", value = task.EmployeeName, inline = true },
                    new { name = "Due Date", value = task.Deadline.ToString("yyyy-MM-dd"), inline = true },
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await httpClient.PostAsync(discordWebhookUrl, content);
        }
    }
}
