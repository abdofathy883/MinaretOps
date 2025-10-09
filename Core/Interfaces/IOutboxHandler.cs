using Core.Models;

namespace Core.Interfaces
{
    public interface IOutboxHandler
    {
        Task HandleAsync(Outbox message, CancellationToken token);
    }
}
