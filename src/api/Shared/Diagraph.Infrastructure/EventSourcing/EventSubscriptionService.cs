using Diagraph.Infrastructure.EventSourcing.Contracts;
using Microsoft.Extensions.Hosting;

namespace Diagraph.Infrastructure.EventSourcing;

public class EventSubscriptionService : IHostedService
{
    private readonly IEnumerable<IEventSubscription> _subscriptions;
    
    public EventSubscriptionService(IEnumerable<IEventSubscription> subscriptions)
        => _subscriptions = subscriptions ?? throw new ArgumentNullException(nameof(subscriptions));

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.WhenAll(_subscriptions.Select(s => s.StartAsync()));

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.WhenAll(_subscriptions.Select(s => s.StopAsync()));
}