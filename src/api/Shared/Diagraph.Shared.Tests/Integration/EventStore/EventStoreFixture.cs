using Diagraph.Infrastructure.Tests;

namespace Diagraph.Shared.Tests.Integration.EventStore;

public class EventStoreFixture : EventStoreModuleFixture
{
    public EventStoreFixture() : base("shared") { }
}