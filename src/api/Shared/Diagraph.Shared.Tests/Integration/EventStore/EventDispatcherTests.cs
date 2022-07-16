using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.Tests.AutoFixture;
using FluentAssertions;

namespace Diagraph.Shared.Tests.Integration.EventStore;

[Collection(nameof(EventStoreCollectionFixture))]
public class EventDispatcherTests
{
    private readonly EventStoreFixture _fixture;

    public EventDispatcherTests(EventStoreFixture fixture) => _fixture = fixture; 
    
    [Theory, CustomizedAutoData]
    public async Task Creates_Stream_On_Writing(string stream, Guid id)
    {
        // Act
        await _fixture.Dispatcher.DispatchAsync(stream, new EventStoreFixture.TestCreateEvent(id));

        // Assert
        List<IEvent> events = await _fixture.Events(stream);
        events.Should().NotBeNullOrEmpty();
        events.Count.Should().Be(1);
        events.Should().ContainItemsAssignableTo<EventStoreFixture.TestCreateEvent>();
        ((EventStoreFixture.TestCreateEvent)events.First()).Id.Should().Be(id);
    }
    
    [Theory, CustomizedAutoData]
    public async Task Append_Event_To_Existing_Stream(string stream, Guid id)
    {
        // Arrange
        await _fixture.Dispatcher.DispatchAsync(stream, new EventStoreFixture.TestCreateEvent(id));
        
        // Act
        await _fixture.Dispatcher.DispatchAsync(stream, new EventStoreFixture.TestCreateEvent(id));

        // Assert
        List<IEvent> events = await _fixture.Events(stream);
        events.Should().NotBeNullOrEmpty();
        events.Count.Should().Be(2);
        events.Should().ContainItemsAssignableTo<EventStoreFixture.TestCreateEvent>();
    }
}