using Core.Enums;
using Core.Models;

namespace Core.DTOs.Payloads
{
    public record DiscordPayload
    (
        string ChannelId,
        TaskItem Task,
        DiscordOperationType OperationType,
        CustomTaskStatus? NewStatus = null
    );
}
