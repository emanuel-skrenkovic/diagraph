using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Hashing;

namespace Diagraph.Modules.Events;

public class Event : DbEntity, IUserRelated
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Text { get; set; }
    
    /// <summary>
    /// Indicates what created the event. E.g. events could be created by the user,
    /// imported from a csv using a custom template, imported from Google Fitness API etc.
    /// </summary>
    public string Source { get; set; }

    public string Discriminator { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public DateTime? EndedAtUtc { get; set; }
    
    public ICollection<EventTag> Tags { get; set; }

    public static Event Create
    (
        IHashTool             hashTool,
        string                text,
        string                source, 
        DateTime              occurredAtUtc,
        DateTime              endedAtUtc,
        IEnumerable<EventTag> tags = null
    )
    {
        Event @event = new()
        {
            Text          = text,
            Source        = source,
            OccurredAtUtc = occurredAtUtc,
            EndedAtUtc    = endedAtUtc, 
            Tags          = tags?.ToList()
        };
        @event.Discriminator = @event.ComputeDiscriminator(hashTool);
        
        return @event;
    }

    public string ComputeDiscriminator(IHashTool hashTool)
        => hashTool.ComputeHash($"{UserId}:{OccurredAtUtc}:{EndedAtUtc ?? DateTime.MinValue}:{Text}");
}