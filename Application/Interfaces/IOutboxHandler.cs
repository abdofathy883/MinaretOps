using Core.Models;

namespace Application.Interfaces
{
    public interface IOutboxHandler
    {
        Task HandleAsync(Outbox message, CancellationToken token);
    }
}
