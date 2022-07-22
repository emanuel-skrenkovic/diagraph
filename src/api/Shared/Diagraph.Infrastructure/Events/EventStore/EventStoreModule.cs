using Diagraph.Infrastructure.EventSourcing.EventStore;
using Diagraph.Infrastructure.Modules;
using EventStore.Client;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Infrastructure.Events.EventStore;

public class EventStoreModule : Module
{
    public override string ModuleName => "event-store";
    
    protected override void RegisterServices
    (
        ApplicationPartManager partManager, 
        IConfiguration         configuration, 
        IServiceCollection     services
    )
    {
        services.AddSingleton
        (
            _ => new EventStoreClient
            (
                EventStoreClientSettings.Create
                (
                    configuration
                        .GetSection(EventStoreConfiguration.SectionName)
                        .Get<EventStoreConfiguration>()
                        .ConnectionString
                )
            )
        );
    }
}