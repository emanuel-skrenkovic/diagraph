using Diagraph.Infrastructure.Hashing;

namespace Diagraph.Modules.Events.Extensions;

public static class EventsExtensions
{
    public static Event WithDiscriminator(this Event @event, IHashTool hashTool)
    {
        @event.Discriminator = @event.ComputeDiscriminator(hashTool);
        return @event;
    }
}