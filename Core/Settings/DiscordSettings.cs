using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Settings
{
    public class DiscordSettings
    {
        public string? BotToken { get; set; }
        public ulong? GuildId { get; set; } // Add this property for the Discord server/guild ID
    }
}
