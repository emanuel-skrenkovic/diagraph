using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using FluentAssertions;

namespace Diagraph.Shared.Tests.Integration.EventStore;

public class EventStoreRepositoryTests : IClassFixture<EventStoreFixture>
{
    private readonly EventStoreFixture _fixture;

    public EventStoreRepositoryTests(EventStoreFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Creates_New_Stream()
    {
        // Arrange
        var repo = _fixture.Repository;

        var agg = TestAggregate.Create();
        agg.SetInt(5);
        agg.SetString(Guid.NewGuid().ToString());
        agg.SetObject(2,Guid.NewGuid().ToString());

        // Act
        await repo.SaveAsync<TestAggregate, Guid>(agg);

        // Assert
        List<IEvent> events = await _fixture.Events(agg.Key());
        events.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task Updates_Existing_Stream_With_Same_Aggregate_Instance()
    {
        // Arrange
        var agg = TestAggregate.Create();
        agg.SetInt(5);
        agg.SetString(Guid.NewGuid().ToString());
        agg.SetObject(2,Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<TestAggregate, Guid>(agg);
        
        // Act
        agg.SetInt(10);
        agg.SetString(Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<TestAggregate, Guid>(agg);

        // Assert
        List<IEvent> events = await _fixture.Events(agg.Key());
        events.Should().NotBeNullOrEmpty();
        events.Count.Should().Be(6);
    }
    
    [Fact]
    public async Task Updates_Existing_Stream_With_Reloaded_Aggregate()
    {
        // Arrange
        var agg = TestAggregate.Create();
        agg.SetInt(5);
        agg.SetString(Guid.NewGuid().ToString());
        agg.SetObject(2,Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<TestAggregate, Guid>(agg);

        var loaded = await _fixture.Repository.LoadAsync<TestAggregate, Guid>(agg.Id);
        
        // Act
        loaded.SetInt(10);
        loaded.SetString(Guid.NewGuid().ToString());
        await _fixture.Repository.SaveAsync<TestAggregate, Guid>(loaded);

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
        var agg = TestAggregate.Create();
        agg.SetInt(5);
        agg.SetString(Guid.NewGuid().ToString());
        agg.SetObject(2, Guid.NewGuid().ToString());

        // Act
        await _fixture.Repository.SaveAsync<TestAggregate, Guid>(agg);

        // Assert
        var loaded = await _fixture.Repository.LoadAsync<TestAggregate, Guid>(agg.Id);
        
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
        var loaded = await _fixture.Repository.LoadAsync<TestAggregate, Guid>(Guid.NewGuid());
        loaded.Should().BeNull();
    }
}

internal record TestCreateEvent(Guid Id): IEvent;
internal record IntSetEvent(int Value): IEvent;
internal record StringSetEvent(string Value): IEvent;
internal record ObjectSetEvent(int TestInt, string TestString) : IEvent;

internal class TestObject 
{
    public int    TestInt    { get; set; }
    public string TestString { get; set; } 
}

internal class TestAggregate : AggregateEntity<Guid>
{
    public int        TestInt    { get; private set; }
    public string     TestString { get; private set; }
    public TestObject TestObject { get; private set; }

    public static TestAggregate Create()
    {
        var agg = new TestAggregate();
        agg.ApplyEvent(new TestCreateEvent(Guid.NewGuid()));
        return agg;
    }

    private void Apply(TestCreateEvent @event) 
        => Id = @event.Id;

    internal void SetInt(int value) 
        => ApplyEvent(new IntSetEvent(value));

    private void Apply(IntSetEvent @event) 
        => TestInt = @event.Value;

    internal void SetString(string value) 
        => ApplyEvent(new StringSetEvent(value));

    private void Apply(StringSetEvent @event) 
        => TestString = @event.Value;

    internal void SetObject(int testInt, string testString) 
        => ApplyEvent(new ObjectSetEvent(testInt, testString));

    private void Apply(ObjectSetEvent @event) 
        => TestObject = new TestObject
        {
            TestInt = @event.TestInt, 
            TestString = @event.TestString
        };

    protected override void RegisterAppliers()
    {
        RegisterApplier<TestCreateEvent>(Apply);
        RegisterApplier<IntSetEvent>(Apply);
        RegisterApplier<StringSetEvent>(Apply);
        RegisterApplier<ObjectSetEvent>(Apply);
    }
}