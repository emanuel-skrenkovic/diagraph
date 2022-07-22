using Diagraph.Infrastructure.EventSourcing.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Infrastructure.ProcessManager.Contracts;

public interface IProcessManager<T> : IEventHandler, IEventSubscription
{
    Task ScheduleAsync(T state);

    public void Schedule<TContext>(T state, TContext uow) where TContext : DbContext;
    
    // TODO: how to know which ID to check?
    Task WakeUpAsync(Process process);
}