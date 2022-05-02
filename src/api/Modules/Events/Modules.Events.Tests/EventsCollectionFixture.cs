using Xunit;

namespace Diagraph.Modules.Events.Tests;

[CollectionDefinition(nameof(EventsCollectionFixture))]
public class EventsCollectionFixture : ICollectionFixture<EventsFixture>
{
}