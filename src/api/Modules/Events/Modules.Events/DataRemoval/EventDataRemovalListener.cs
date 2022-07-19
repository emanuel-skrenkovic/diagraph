using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Events.Contracts;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Modules.Events.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Integration.UserData.Events;

namespace Diagraph.Modules.Events.DataRemoval;

public class EventDataRemovalListener : IEventSubscription, IEventHandler
{
    private readonly IEventDispatcher     _dispatcher;
    private readonly EventSubscriber      _subscriber;
    private readonly IServiceScopeFactory _scopeFactory;

    public EventDataRemovalListener
    (
        IEventDispatcher dispatcher,
        EventSubscriber subscriber, 
        IServiceScopeFactory scopeFactory
    )
    {
        _dispatcher   = dispatcher;
        _subscriber   = subscriber;
        _scopeFactory = scopeFactory;
    }
    
    public async Task HandleAsync(IEvent @event, EventMetadata metadata)
    {
        IServiceScope scope     = _scopeFactory.CreateScope();
        EventsDbContext context = scope.GetContext<EventsDbContext>();

        if (@event is not RequestedEventDataRemovalEvent removalEvent) return;

        List<Event> userEvents = await context
            .Events
            .Where(e => e.UserId == Guid.Parse(removalEvent.UserName))
            .ToListAsync();

        if (!userEvents.Any())
        {
            // Dispatch the event either way so the process manager doesn't get stuck.
            await _dispatcher.DispatchAsync
            (
                "TODO", 
                new EventDataRemovedEvent(removalEvent.UserName)
            );
            return;
        }

        context.Events.RemoveRange(userEvents);
        await context.SaveChangesAsync();

        await _dispatcher.DispatchAsync
        (
            "TODO", 
            new EventDataRemovedEvent(removalEvent.UserName)
        );
    }
    
    public Task StartAsync() => _subscriber.SubscribeAsync(HandleAsync);

    public Task StopAsync() => Task.CompletedTask;
}