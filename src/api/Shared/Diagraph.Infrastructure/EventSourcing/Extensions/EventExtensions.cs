using System.Text.Json;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.Guids;
using EventStore.Client;

namespace Diagraph.Infrastructure.EventSourcing.Extensions;

public static class EventExtensions
{
    public static object ToEventObject(this ResolvedEvent resolvedEvent)
    {
        EventRecord record = resolvedEvent.Event;

        return JsonSerializer.Deserialize
        (
            record.Data.ToArray(),
            Type.GetType(record.EventType)!
        );
    }
    
    public static IEvent ToEvent(this ResolvedEvent resolvedEvent)
    {
        EventRecord record = resolvedEvent.Event;

        return (IEvent) JsonSerializer.Deserialize
        (
            record.Data.ToArray(),
            Type.GetType(record.EventType)!
        );
    }
    
    public static EventData ToEventData<T>(this T domainEvent, EventMetadata metadata) 
    {
        Type eventType = domainEvent.GetType();
            
        return new EventData
        (
            eventId:  Uuid.FromGuid(metadata.EventId), 
            type:     eventType.AssemblyQualifiedName!,
            data:     JsonSerializer.SerializeToUtf8Bytes(domainEvent, eventType),
            metadata: JsonSerializer.SerializeToUtf8Bytes(metadata)
        );
    }

    public static EventMetadata Metadata(this ResolvedEvent resolvedEvent)
        => JsonSerializer.Deserialize<EventMetadata>(resolvedEvent.Event.Metadata.ToArray());

    // TODO: think about if this is needed.
    public static EventMetadata Metadata(this IEvent @event, ICorrelationContext context)
        => new()
        {
            EventId = DeterministicGuid.New
            (
                @namespace: @event.GetType().FullName,
                name:       context.MessageId + @event.GetType().FullName
            ),
            CorrelationId = context.CorrelationId,
            CausationId   = context.CausationId
        };
}