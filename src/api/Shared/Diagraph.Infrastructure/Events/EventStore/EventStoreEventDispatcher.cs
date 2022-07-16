using Diagraph.Infrastructure.Events.Contracts;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.EventSourcing.Extensions;
using EventStore.Client;

namespace Diagraph.Infrastructure.Events.EventStore;

public class EventStoreEventDispatcher : IEventDispatcher
{
    private readonly ICorrelationContext _correlationContext;
    private readonly EventStoreClient    _client;

    public EventStoreEventDispatcher(ICorrelationContext correlationContext, EventStoreClient client)
    {
        _correlationContext = correlationContext;
        _client             = client;
    }
    
    public async Task DispatchAsync(string stream, IEvent @event)
    {
        Ensure.NotNullOrEmpty(stream);
        Ensure.NotNull(@event);
        
        await _client.AppendToStreamAsync
        (
            stream, 
            StreamState.Any, 
            new[]
            {
                @event.ToEventData(@event.Metadata(_correlationContext))
            }
        );
    }
}