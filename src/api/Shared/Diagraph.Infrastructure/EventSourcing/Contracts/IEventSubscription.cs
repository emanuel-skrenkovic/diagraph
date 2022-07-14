namespace Diagraph.Infrastructure.EventSourcing.Contracts;

public interface IEventSubscription
{
    Task StartAsync();

    Task StopAsync();
}