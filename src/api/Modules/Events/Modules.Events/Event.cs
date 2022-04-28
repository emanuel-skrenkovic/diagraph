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
    
    public string CustomData { get; set; }
    
    public ICollection<EventTag> Tags { get; set; }

    public static Event Create
    (
        IHashTool             hashTool,
        Guid                  userId,
        string                text,
        DateTime              occurredAtUtc,
        IEnumerable<EventTag> tags,
        string                customData = null
    )
    {
        Event @event = new()
        {
            UserId        = userId,
            Text          = text,
            OccurredAtUtc = occurredAtUtc,
            CustomData    = customData,
            Tags          = tags.ToList()
        };
        @event.Discriminator = @event.ComputeDiscriminator(hashTool);
        
        return @event;
    }

    public string ComputeDiscriminator(IHashTool hashTool)
        => hashTool.ComputeHash($"{UserId}:{OccurredAtUtc}:{Text}");

}