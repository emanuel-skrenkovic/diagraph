namespace Diagraph.Infrastructure.EventSourcing.Contracts;

public class EventMetadata
{
    public ulong StreamPosition { get; set; }
    
    public Guid EventId { get; set; }
    
    public Guid CorrelationId { get; set; }
    
    public Guid CausationId { get; set; }
}