namespace Diagraph.Infrastructure.EventSourcing.Contracts;

public interface IEventHandler
{
    Task HandleAsync(IEvent @event, EventMetadata metadata);
}