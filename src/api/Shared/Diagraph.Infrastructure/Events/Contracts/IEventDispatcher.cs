using Diagraph.Infrastructure.EventSourcing.Contracts;

namespace Diagraph.Infrastructure.Events.Contracts;

public interface IEventDispatcher
{
    Task DispatchAsync(string stream, IEvent @event);
}