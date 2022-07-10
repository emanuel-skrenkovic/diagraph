using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.EventSourcing.Extensions;
using Diagraph.Infrastructure.Retries;
using EventStore.Client;

// ReSharper disable AsyncVoidLambda

namespace Diagraph.Infrastructure.EventSourcing;

public class EventStreamSubscriber
{
    // TODO: values from configuration.
    private readonly RetryCounter _retryCounter = new
    (
        new RetryConfiguration { MaxRetryCount = 3, RetryDelayMilliseconds = 500 }
    );

    private readonly EventStoreClient _client;

    public EventStreamSubscriber(EventStoreClient client) => _client = client;

    public async Task SubscribeAsync
    (
        Func<IEvent, EventMetadata, Task> onEvent,
        string                            streamName,
        ulong                             position = 0UL
    )
    {
        await _client.SubscribeToStreamAsync
        (
            streamName:    streamName,
            start:         FromStream.After(StreamPosition.FromStreamRevision(position)),
            eventAppeared: async (_, resolvedEvent, _)
                => await onEvent(resolvedEvent.ToEvent(), resolvedEvent.Metadata()),
            subscriptionDropped: SubscriptionDroppedHandler(onEvent, streamName, position)
        );
    }

    private Action<StreamSubscription, SubscriptionDroppedReason, Exception> SubscriptionDroppedHandler
    (
        Func<IEvent, EventMetadata, Task> onEvent,
        string                            streamName,
        ulong                             position
    )
    {
        return async (_, reason, exception) =>
        {
            if (reason is SubscriptionDroppedReason.Disposed)
            {
                if (exception is not null) throw exception;
                throw new Exception(reason.ToString()); // TODO: exception type.
            }

            await _retryCounter.RetryAsync
            (
                () => _client.SubscribeToStreamAsync
                (
                    streamName:    streamName,
                    start:         FromStream.After(StreamPosition.FromStreamRevision(position)),
                    eventAppeared: async (_, resolvedEvent, _)
                        => await onEvent(resolvedEvent.ToEvent(), resolvedEvent.Metadata()),
                    subscriptionDropped: SubscriptionDroppedHandler(onEvent, streamName, position)
                )
            );
        };
    }
}