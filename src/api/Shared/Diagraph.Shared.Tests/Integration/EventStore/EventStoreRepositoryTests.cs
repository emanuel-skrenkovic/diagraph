using Diagraph.Infrastructure.EventSourcing.Contracts;
using FluentAssertions;

namespace Diagraph.Shared.Tests.Integration.EventStore;

[Collection(nameof(EventStoreCollectionFixture))]
public class EventStoreRepositoryTests
{
    private readonly EventStoreFixture _fixture;

    public EventStoreRepositoryTests(EventStoreFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Creates_New_Stream()
    {
        // Arrange
        var repo = _fixture.Repository;

        var agg = EventStoreFixture.TestAggregate.Create();
        agg.SetInt(5);
        agg.SetString(Guid.NewGuid().ToString());
        agg.SetObject(2, Guid.NewGuid().ToString());

        // Act
        await repo.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);

        // Assert
        List<IEvent> events = await _fixture.Events(agg.Key());
        events.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Updates_Existing_Stream_With_Same_Aggregate_Instance()
    {
        // Arrange
        var agg = EventStoreFixture.TestAggregate.Create();
        agg.SetInt(5);
        agg.SetString(Guid.NewGuid().ToString());
        agg.SetObject(2, Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);

        // Act
        agg.SetInt(10);
        agg.SetString(Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);

        // Assert
        List<IEvent> events = await _fixture.Events(agg.Key());
        events.Should().NotBeNullOrEmpty();
        events.Count.Should().Be(6);
    }

    [Fact]
    public async Task Updates_Existing_Stream_With_Reloaded_Aggregate()
    {
        // Arrange
        var agg = EventStoreFixture.TestAggregate.Create();
        agg.SetInt(5);
        agg.SetString(Guid.NewGuid().ToString());
        agg.SetObject(2, Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);

        var loaded = await _fixture.Repository.LoadAsync<EventStoreFixture.TestAggregate, Guid>(agg.Id);

        // Act
        loaded.SetInt(10);
        loaded.SetString(Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(loaded);

        // Assert
        loaded.Key().Should().Be(agg.Key());

        List<IEvent> events = await _fixture.Events(agg.Key());
        events.Should().NotBeNullOrEmpty();
        events.Count.Should().Be(6);
    }

    [Fact]
    public async Task Loads_Existing_Aggregate()
    {
        // Arrange
        var agg = EventStoreFixture.TestAggregate.Create();
        agg.SetInt(5);
        agg.SetString(Guid.NewGuid().ToString());
        agg.SetObject(2, Guid.NewGuid().ToString());

        // Act
        await _fixture.Repository.SaveAsync<EventStoreFixture.TestAggregate, Guid>(agg);

        // Assert
        var loaded = await _fixture.Repository.LoadAsync<EventStoreFixture.TestAggregate, Guid>(agg.Id);

        loaded.Should().NotBeNull();
        loaded.TestInt.Should().Be(agg.TestInt);
        loaded.TestString.Should().Be(agg.TestString);

        loaded.TestObject.Should().NotBeNull();
        loaded.TestObject.TestInt.Should().Be(agg.TestObject.TestInt);
        loaded.TestObject.TestString.Should().Be(agg.TestObject.TestString);
    }

    [Fact]
    public async Task Returns_Null_If_Stream_Does_Not_Exist()
    {
        var loaded = await _fixture.Repository.LoadAsync<EventStoreFixture.TestAggregate, Guid>(Guid.NewGuid());
        loaded.Should().BeNull();
    }
}