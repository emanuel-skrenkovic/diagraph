using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.EventSourcing.Extensions;
using EventStore.Client;

namespace Diagraph.Infrastructure.EventSourcing;

public class EventStoreAggregateRepository : IAggregateRepository
{
    // TODO: ICorrelationContext?
    private readonly ICorrelationContext _context;
    private readonly EventStoreClient    _client;
    
    public EventStoreAggregateRepository
    (
        ICorrelationContext context, 
        EventStoreClient client
    )
    {
        _context = context;
        _client  = client;
    }

    public async Task SaveAsync<T, TKey>(T aggregate) where T : AggregateEntity<TKey>
    {
        Ensure.NotNull(aggregate);

        IEnumerable<EventData> events = aggregate
            .GetUncommittedEvents()
            .Select(e => e.ToEventData(e.Metadata(_context)));
        
        await _client.AppendToStreamAsync
        (
            StreamName<T, TKey>(aggregate.Id),
            StreamState.Any,
            events
        );
    }

    public async Task<T> LoadAsync<T, TKey>(TKey key) 
        where T : AggregateEntity<TKey>, new()
    {
        Ensure.NotNull(key);
        
        EventStoreClient.ReadStreamResult eventStream = _client.ReadStreamAsync
        (
            Direction.Forwards, 
            StreamName<T, TKey>(key),
            StreamPosition.Start
        );
        if (await eventStream.ReadState == ReadState.StreamNotFound) return null;

        IReadOnlyCollection<IEvent> events = await eventStream
            .Select(e => e.ToEvent())
            .ToArrayAsync();

        T aggregate = new();
        aggregate.Hydrate(key, events);
        return aggregate;
    }
    
    private string StreamName<T, TKey>(TKey id) => $"{typeof(T).FullName}-{id}";
}