using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.ProcessManager.Contracts;
using Diagraph.Modules.Identity.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Integration.UserData.Events;

namespace Diagraph.Modules.Identity.UserData;

/// <summary>
/// This process manager holds the state for use data deletion. The data is located in multiple
/// modules, possibly multiple databases so a coordinator is required.
/// This should be triggered by a background job. In case a certain, configured amount of time passes
/// between sending a request to delete data, and the data is not marked as deleted, the process manager
/// should be woken up and the data deletion request sent again.
/// This is all done asynchronously, the mechanism used is the event stream located in EventStore.
/// </summary>
public class UserDataDeletionProcessManager : IProcessManager<UserDataRemovalState> 
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EventSubscriber      _subscriber;
    private readonly IAggregateRepository _repository;
    
    string SubscriptionId => $"{typeof(UserDataRemovalState).AssemblyQualifiedName}";

    public UserDataDeletionProcessManager
    (
        IServiceScopeFactory           scopeFactory,
        EventSubscriber                subscriber,
        IAggregateRepository           repository
    )
    {
        _scopeFactory = scopeFactory;
        _subscriber   = subscriber;
        _repository   = repository;
    }

    public async Task ScheduleAsync(UserDataRemovalState state)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IdentityDbContext context = scope.GetContext<IdentityDbContext>();
        
        Process process = new() { ProcessId = state.Id }; 
        process.SetData(state);
        context.Add(process);

        // TODO: should this be here? I really need to clean everything up.
        await _repository.SaveAsync<UserDataRemovalState, string>(state);
        await context.SaveChangesAsync();
    }

    public void Schedule<TContext>(UserDataRemovalState state, TContext uow)
        where TContext : DbContext
    {
        Process process = new() { ProcessId = state.Id }; 
        process.SetData(state);
        uow.Add(process);
    }

    public async Task HandleAsync(IEvent @event, EventMetadata metadata)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IdentityDbContext context = scope.GetContext<IdentityDbContext>();

        // Correlation, Causation, etc.
        Func<UserDataRemovalState> handleEvent = @event switch
        {
            EventDataRemovedEvent eventDataDeletedEvent => await WithState
            (
                eventDataDeletedEvent.UserName,
                state => 
                {
                    if (state.DataRemoved) state.RequestUserAccountRemoval();
                    return state;
                }
            ),
            GlucoseDataRemovedEvent glucoseDataDeleted => await WithState
            (
                glucoseDataDeleted.UserName, 
                state =>
                {
                    if (state.DataRemoved) state.RequestUserAccountRemoval();
                    return state; 
                }
            ),
            _ => null
        };

        if (handleEvent is null) return;
        
        SubscriptionCheckpoint checkpoint = await context
            .Checkpoints
            .SingleOrDefaultAsync(c => c.SubscriptionId == SubscriptionId);

        if (checkpoint is null)
        {
            checkpoint = new()
            {
                SubscriptionId = SubscriptionId, Position = metadata.StreamPosition
            };            
        }

        UserDataRemovalState newState = handleEvent();
        await _repository.SaveAsync<UserDataRemovalState, string>(newState);
        
        Process process = await context
            .Processes
            .SingleAsync(p => p.ProcessId == newState.Id);
        process.SetData(newState);
        
        context.Update(process);
        context.Update(checkpoint);

        await context.SaveChangesAsync();
    }

    // This is essentially the outbox pattern. The process manager is periodically woken up,
    // and requests are retried if they are not deemed to be successful. Idempotency is key.
    public async Task WakeUpAsync(Process process)
    {
        Ensure.NotNull(process);
        
        using IServiceScope scope = _scopeFactory.CreateScope();
        IdentityDbContext context = scope.GetContext<IdentityDbContext>();
        
        // TODO: have the process entity with subscription id and maybe some initial state.
        // Also, have it be a dynamic data container.
        UserDataRemovalState state = process.GetData<UserDataRemovalState>();
        
        if (!process.Initiated)
        {
            state.RequestRemoval();
            state.RequestEventDataRemoval();
            state.RequestGlucoseDataRemoval();
        
            await _repository.SaveAsync<UserDataRemovalState, string>(state);

            process.Initiated = true;
            process.SetData(state);
            
            context.Update(process);
            await context.SaveChangesAsync();
            return;
        }

        if (state.RequestedEventDataRemoval && !state.EventDataRemoved)
            state.RequestEventDataRemoval();
        
        if (state.RequestedGlucoseDataRemoval && !state.GlucoseDataRemoved)
            state.RequestEventDataRemoval(); 

        if (state.RequestedAccountRemoval && !state.AccountRemoved)
            state.RequestUserAccountRemoval();

        if (state.UserRemoved && !process.Finished)
            process.Finished = true;

        await _repository.SaveAsync<UserDataRemovalState, string>(state);
        context.Update(state);
        await context.SaveChangesAsync();
    }

    private async Task<Func<UserDataRemovalState>> WithState
    (
        string userName, 
        Func<UserDataRemovalState, UserDataRemovalState> action
    )
    {
        UserDataRemovalState state = await _repository.LoadAsync<UserDataRemovalState, string>(userName);
        return () => action(state);
    }

    #region IEventSubscription
    
    public async Task StartAsync()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IdentityDbContext context = scope.GetContext<IdentityDbContext>();

        SubscriptionCheckpoint checkpoint = await context.GetOrAddAsync
        (
            c => c.SubscriptionId == typeof(UserDataRemovalState).AssemblyQualifiedName,
            new SubscriptionCheckpoint { SubscriptionId = SubscriptionId, Position = 0 }
        );

        await _subscriber.SubscribeAsync(HandleAsync, checkpoint.Position);
    }

    public Task StopAsync() => Task.CompletedTask;

    #endregion
}