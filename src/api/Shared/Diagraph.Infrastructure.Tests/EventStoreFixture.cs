using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Api;
using Diagraph.Infrastructure.Events.Contracts;
using Diagraph.Infrastructure.Events.EventStore;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.EventSourcing.EventStore;
using Diagraph.Infrastructure.EventSourcing.Extensions;
using Diagraph.Infrastructure.Tests.Docker;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Diagraph.Infrastructure.Tests;

public class EventStoreFixture : IAsyncLifetime
{
    private readonly EventStoreContainer _container;
    
    private CorrelationContext  CorrelationContext => new()
    {
        CausationId   = Guid.NewGuid(),
        CorrelationId = Guid.NewGuid(),
        MessageId     = Guid.NewGuid() 
    };
    
    public EventSubscriber Subscriber => new
    (
        CorrelationContext,
        _container.EventStore
    );

    public IEventDispatcher Dispatcher => new EventStoreEventDispatcher
    (
        CorrelationContext,
        _container.EventStore
    );
    
    // TODO
    public IAggregateRepository Repository => new EventStoreAggregateRepository
    (
        CorrelationContext,
        _container.EventStore
    );

    public EventStoreFixture()
    {
         IConfiguration configuration = new ConfigurationManager()
             .AddJsonFile($"appsettings.integration-test.json") // TODO: need better way to configure which settings file to use
             .Build();
 
         _container = new EventStoreContainer
         (
             configuration
                 .GetSection(EventStoreConfiguration.SectionName)
                 .Get<EventStoreConfiguration>()
                 .ConnectionString
         );
    }

    public ValueTask<List<IEvent>> Events(string stream)
        => _container
            .EventStore
            .ReadStreamAsync(Direction.Forwards, stream, StreamPosition.Start)
            .Select(e => e.ToEvent())
            .ToListAsync();

    public async Task DispatchEvent(string stream, IEvent @event)
    {
        EventMetadata metadata = @event.Metadata(CorrelationContext);
        EventData eventData    = @event.ToEventData(metadata);
        
        await _container.EventStore.AppendToStreamAsync
        (
            stream,
            StreamState.Any,
            new[] { eventData }
        ); 
    }
    
    #region IAsyncLifetime

    public Task InitializeAsync() => _container.InitializeAsync();

    public Task DisposeAsync() => _container.DisposeAsync();
    
    #endregion
}