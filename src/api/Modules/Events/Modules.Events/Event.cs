using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Hashing;

namespace Diagraph.Modules.Events;

public class Event : DbEntity, IUserRelated
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Text { get; set; }

    public string Discriminator { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public DateTime? EndedAtUtc { get; set; }
    
    public ICollection<EventTag> Tags { get; set; }

    public static Event Create
    (
        IHashTool             hashTool,
        string                text,
        DateTime              occurredAtUtc,
        DateTime              endedAtUtc,
        IEnumerable<EventTag> tags = null
    )
    {
        Event @event = new()
        {
            Text          = text,
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