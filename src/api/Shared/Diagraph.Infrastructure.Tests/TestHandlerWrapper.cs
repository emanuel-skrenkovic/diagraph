using System;
using System.Threading.Tasks;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;

namespace Diagraph.Infrastructure.Tests;

public class TestHandlerWrapper
{
    private readonly EventSubscriber               _subscriber;
    private readonly IEventHandler                 _handler;
    private readonly Action<IEvent, EventMetadata> _afterEvent;

    public TestHandlerWrapper
    (
        EventSubscriber               subscriber, 
        IEventHandler                 handler, 
        Action<IEvent, EventMetadata> afterEvent
    )
    {
        _subscriber = subscriber;
        _handler    = handler;
        _afterEvent = afterEvent;
    }

    public static Task RunAsync
    ( 
        EventSubscriber               subscriber, 
        IEventHandler                 handler, 
        Action<IEvent, EventMetadata> afterEvent
    )
        => new TestHandlerWrapper(subscriber, handler, afterEvent).StartAsync();

    public Task StartAsync() => _subscriber.SubscribeAsync(HandleAsync);

    public Task StopAsync() => Task.CompletedTask;

    public async Task HandleAsync(IEvent @event, EventMetadata metadata)
    {
        await _handler.HandleAsync(@event, metadata);
        _afterEvent(@event, metadata);
    }
}