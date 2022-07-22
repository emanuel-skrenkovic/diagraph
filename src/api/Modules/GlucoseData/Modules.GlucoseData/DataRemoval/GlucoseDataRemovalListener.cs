using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Events.Contracts;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Modules.GlucoseData.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Integration.UserData;
using Modules.Identity.Integration.UserData.Events;

namespace Diagraph.Modules.GlucoseData.DataRemoval;

public class GlucoseDataRemovalListener : IEventSubscription, IEventHandler
{
    private readonly IEventDispatcher     _dispatcher;
    private readonly EventSubscriber      _subscriber;
    private readonly IServiceScopeFactory _scopeFactory;

    public GlucoseDataRemovalListener
    (
        IEventDispatcher     dispatcher,
        EventSubscriber      subscriber,
        IServiceScopeFactory scopeFactory
    )
    {
        _dispatcher   = dispatcher;
        _subscriber   = subscriber;
        _scopeFactory = scopeFactory;
    }
    
    public async Task HandleAsync(IEvent @event, EventMetadata metadata)
    {
        IServiceScope        scope   = _scopeFactory.CreateScope();
        GlucoseDataDbContext context = scope.GetContext<GlucoseDataDbContext>();

        if (@event is not RequestedGlucoseDataRemovalEvent removalEvent) return;

        List<GlucoseMeasurement> measurements = await context
            .GlucoseMeasurements
            .Where(e => e.UserId == Guid.Parse(removalEvent.UserName))
            .ToListAsync();

        if (!measurements.Any())
        {
            // Dispatch the event either way so the process manager doesn't get stuck.
            await _dispatcher.DispatchAsync
            (
                UserDataRemovalConsts.IntegrationStreamName,
                new GlucoseDataRemovedEvent(removalEvent.UserName)
            );
            return;
        }

        context.GlucoseMeasurements.RemoveRange(measurements);
        await context.SaveChangesAsync();

        await _dispatcher.DispatchAsync
        (
            UserDataRemovalConsts.IntegrationStreamName,
            new GlucoseDataRemovedEvent(removalEvent.UserName)
        ); 
    }
    
    public Task StartAsync() => _subscriber.SubscribeAsync(HandleAsync);

    public Task StopAsync() => Task.CompletedTask;
}