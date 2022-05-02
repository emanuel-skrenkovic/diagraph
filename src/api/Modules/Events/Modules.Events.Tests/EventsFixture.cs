using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.Events.Database;

namespace Diagraph.Modules.Events.Tests;

public class EventsFixture : DatabaseModuleFixture<EventsDbContext>
{
    public EventsFixture() : base("events") { }
}