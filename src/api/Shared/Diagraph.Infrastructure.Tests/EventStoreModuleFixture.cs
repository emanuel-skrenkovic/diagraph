using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Api;
using Diagraph.Infrastructure.Events.Contracts;
using Diagraph.Infrastructure.Events.EventStore;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.EventSourcing.Extensions;
using Diagraph.Infrastructure.Tests.Docker;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Diagraph.Infrastructure.Tests;

public class EventStoreModuleFixture : IAsyncLifetime
{
    public readonly EventStoreContainer EventStore;
    
    public EventSubscriber Subscriber => new
    (
        new CorrelationContext
        {
            CausationId   = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            MessageId     = Guid.NewGuid()
        },
        EventStore.EventStore
    );

    public IEventDispatcher Dispatcher => new EventStoreEventDispatcher
    (
        new CorrelationContext
        {
            CausationId   = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            MessageId     = Guid.NewGuid()
        },
        EventStore.EventStore
    );
    
    // TODO
    public IAggregateRepository Repository => new EventStoreAggregateRepository
    (
        new CorrelationContext
        {
            CausationId   = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            MessageId     = Guid.NewGuid()
        },
        EventStore.EventStore
    );

    public EventStoreModuleFixture(string moduleName)
    {
         IConfiguration configuration = new ConfigurationManager()
             .AddJsonFile($"module.{moduleName}.integration-test.json")
             .Build();
 
         EventStore = new EventStoreContainer(configuration["EventStoreConfiguration:ConnectionString"]);       
    }

    public ValueTask<List<IEvent>> Events(string stream)
        => EventStore
            .EventStore
            .ReadStreamAsync(Direction.Forwards, stream, StreamPosition.Start)
            .Select(e => e.ToEvent())
            .ToListAsync();

    public Task InitializeAsync() => EventStore.InitializeAsync();

    public Task DisposeAsync() => EventStore.DisposeAsync();
}