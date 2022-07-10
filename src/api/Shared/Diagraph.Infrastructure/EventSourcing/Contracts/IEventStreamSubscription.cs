namespace Diagraph.Infrastructure.EventSourcing.Contracts;

public interface IEventStreamSubscription
{
    Task StartAsync();

    Task StopAsync();
}