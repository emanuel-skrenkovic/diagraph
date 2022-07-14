using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using FluentAssertions;
// ReSharper disable CoVariantArrayConversion

namespace Diagraph.Shared.Tests.Integration.EventStore;

[Collection(nameof(EventStoreCollectionFixture))]
public class EventStoreSubscriptionTests
{
    private readonly EventStoreFixture _fixture;

    public EventStoreSubscriptionTests(EventStoreFixture fixture) => _fixture = fixture;
    
    [Fact]
    public async Task Subscribes_Successfully()
    {
        // Arrange
        var subscription = new TestSubscription
        (
            (_, _) => Task.CompletedTask,
            _fixture.Subscriber
        );
        
        // Act
        await subscription.StartAsync();
    }
    
    [Fact]
    public async Task Reads_Event_From_Stream()
    {
        // Arrange
        AutoResetEvent handle = new AutoResetEvent(false);
        
        var agg = EventStoreFixture.TestAggregate.Create();
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);
        
        var subscription = new TestSubscription
        (
            (e, _) =>
            {
                e.Should().BeOfType<EventStoreFixture.TestCreateEvent>();
                handle.Set();
                return Task.CompletedTask;
            }, 
            _fixture.Subscriber
        );
        
        // Act
        await subscription.StartAsync();
    
        handle.WaitOne().Should().BeTrue();
    }
    
    [Fact]
    public async Task Reads_Multiple_Events_From_Stream_After_Subscribing()
    {
        // Arrange
        AutoResetEvent[] handles = { new(false), new(false), new(false) };
        
        var agg = EventStoreFixture.TestAggregate.Create();
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);

        int counter = 0;
        
        var subscription = new TestSubscription
        (
            (_, _) =>
            {
                handles[counter++].Set();
                return Task.CompletedTask;
            }, 
            _fixture.Subscriber
        );
        
        // Act
        await subscription.StartAsync();
        
        agg.SetInt(10);
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);
        agg.SetString(Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);
        
        // Assert
        WaitHandle.WaitAll(handles).Should().BeTrue();
    }
    
    [Fact]
    public async Task Reads_All_Existing_Events_From_Stream()
    {
        // Arrange
        AutoResetEvent[] handles = { new(false), new(false), new(false) };
        
        var agg = EventStoreFixture.TestAggregate.Create();
        agg.SetInt(10);
        agg.SetString(Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);

        int counter = 0;
        
        var subscription = new TestSubscription
        (
            (_, _) =>
            {
                handles[counter++].Set();
                return Task.CompletedTask;
            }, 
            _fixture.Subscriber
        );
        
        // Act
        await subscription.StartAsync();
        
        // Assert
        WaitHandle.WaitAll(handles).Should().BeTrue();
    }
}

internal class TestSubscription : IEventSubscription, IEventHandler
{
    private readonly Func<IEvent, EventMetadata, Task> _onEvent;
    private readonly EventSubscriber                   _subscriber;

    public TestSubscription
    (
        Func<IEvent, EventMetadata, Task> onEvent,
        EventSubscriber                   subscriber
    )
    {
        _onEvent    = onEvent;
        _subscriber = subscriber;   
    }
    
    public Task StartAsync() => _subscriber.SubscribeAsync(_onEvent);

    public Task StopAsync() => Task.CompletedTask;
    
    public Task HandleAsync(IEvent @event, EventMetadata metadata) => _onEvent(@event, metadata);
}