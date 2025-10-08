using Core.DTOs.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Payloads
{
    public record DiscordPayload
    (
        string ChannelId,
        TaskDTO Task
    );
}
