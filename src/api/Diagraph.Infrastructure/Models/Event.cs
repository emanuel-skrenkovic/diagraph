using Diagraph.Core.Database;
using Diagraph.Infrastructure.Hashing;

namespace Diagraph.Infrastructure.Models;

public class Event : DbEntity, IUserRelated
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Text { get; set; }

    public string Discriminator { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public string CustomData { get; set; }
    
    public ICollection<EventTag> Tags { get; set; }

    public static Event Create(IHashTool hashTool)
    {
        throw new NotImplementedException();
    }

    public string ComputeDiscriminator(IHashTool hashTool)
        => hashTool.ComputeHash($"{UserId}:{OccurredAtUtc}:{Text}");

}