using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.ProcessManager.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Diagraph.Infrastructure.ProcessManager;

public class WakeUpManager<TContext, T> : IJob 
    where TContext : DbContext 
    where T: class
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IProcessManager<T>   _processManager;


    public WakeUpManager(IServiceScopeFactory scopeFactory, IProcessManager<T> processManager)
    {
        _scopeFactory   = scopeFactory;
        _processManager = processManager;
    } 

    private async Task WakeUpProcesses()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        TContext context          = scope.GetContext<TContext>();
        
        IAsyncEnumerable<Process> waitingProcesses = context
            .Set<Process>()
            .Where(p => !p.Initiated 
                        || (!p.Finished && DateTime.UtcNow > p.UpdatedAtUtc.AddMinutes(5))) // TODO: rules?
            .AsAsyncEnumerable();

        await foreach (Process process in waitingProcesses)
        {
            await _processManager.WakeUpAsync(process);
        }
    }
    
    #region IJob

    public Task Execute(IJobExecutionContext context) => WakeUpProcesses();
    
    #endregion
}