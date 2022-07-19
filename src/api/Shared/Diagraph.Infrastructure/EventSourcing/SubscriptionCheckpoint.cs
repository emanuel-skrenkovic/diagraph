namespace Diagraph.Infrastructure.EventSourcing;

public class SubscriptionCheckpoint
{
    public int Id { get; set; }
    
    public string SubscriptionId { get; set; }
    
    public ulong Position { get; set; }
}