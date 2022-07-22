using Diagraph.Infrastructure.Api;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.EventSourcing.Extensions;
using Diagraph.Infrastructure.Retries;
using EventStore.Client;

// ReSharper disable AsyncVoidLambda

namespace Diagraph.Infrastructure.EventSourcing;

// TODO: maybe avoid retries altogether. Keep the failed state somewhere,
// and have a job which starts all the failed subscribers?
public class EventSubscriber : IDisposable
{
    // TODO: values from configuration.
    private readonly RetryCounter _retryCounter = new(RetryConfiguration.Default);

    private readonly CorrelationContext _correlationContext;
    private readonly EventStoreClient   _client;

    private StreamSubscription _subscription;

    public EventSubscriber(ICorrelationContext correlationContext, EventStoreClient client)
    {
        _correlationContext = correlationContext as CorrelationContext;
        _client             = client;   
    }

    public async Task SubscribeAsync(Func<IEvent, EventMetadata, Task> onEvent, ulong checkpoint = 0UL)
    {
        await _retryCounter.RetryAsync(async () =>
        {
            _subscription = await _client.SubscribeToAllAsync
            (
                start: FromAll.After(new Position(checkpoint, checkpoint)),
                eventAppeared: async (_, resolvedEvent, _) =>
                {
                    IEvent @event = resolvedEvent.ToEventObject() as IEvent;
                    if (@event is null) return;

                    EventMetadata metadata = resolvedEvent.Metadata();

                    _correlationContext.CorrelationId = metadata.CorrelationId;
                    _correlationContext.CausationId   = metadata.EventId;
                    _correlationContext.MessageId     = Guid.NewGuid();

                    await onEvent(@event, metadata);
                },
                subscriptionDropped: SubscriptionDroppedHandler(onEvent, checkpoint),
                filterOptions: new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents())
            );
        });
    }

    private Action<StreamSubscription, SubscriptionDroppedReason, Exception> SubscriptionDroppedHandler
    (
        Func<IEvent, EventMetadata, Task> onEvent,
        ulong                             checkpoint
    ) => async (_, reason, exception) =>
    {
        if (reason is SubscriptionDroppedReason.Disposed)
        {
            if (exception is not null) throw exception;
            throw new Exception(reason.ToString()); // TODO: exception type.
        }
        
        await SubscribeAsync(onEvent, checkpoint);
    };

    #region IDisposable
    
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _client.Dispose();
            _subscription.Dispose();
        }

        _disposed = true; 
    }

    ~EventSubscriber() =>Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    #endregion
}