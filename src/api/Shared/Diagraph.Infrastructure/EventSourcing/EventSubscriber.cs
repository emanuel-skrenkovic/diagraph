using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.EventSourcing.Extensions;
using Diagraph.Infrastructure.Retries;
using EventStore.Client;

// ReSharper disable AsyncVoidLambda

namespace Diagraph.Infrastructure.EventSourcing;

public class EventSubscriber
{
    // TODO: values from configuration.
    private readonly RetryCounter _retryCounter = new
    (
        new RetryConfiguration { MaxRetryCount = 3, RetryDelayMilliseconds = 500 }
    );

    private readonly EventStoreClient _client;

    public EventSubscriber(EventStoreClient client) => _client = client;
    
    public Task SubscribeAsync(Func<IEvent, EventMetadata, Task> onEvent, ulong checkpoint = 0UL)
        => _client.SubscribeToAllAsync
        (
            start: FromAll.After(new Position(checkpoint, checkpoint)),
            // TODO: what if we can't deserialize into IEvent
            eventAppeared: async (_, resolvedEvent, _)
                => await onEvent(resolvedEvent.ToEvent(), resolvedEvent.Metadata()),
            subscriptionDropped: SubscriptionDroppedHandler(onEvent, checkpoint),
            filterOptions: new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents())
        );

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

        await _retryCounter.RetryAsync(() => SubscribeAsync(onEvent, checkpoint));
    };
}