namespace Diagraph.Infrastructure.EventSourcing.Contracts;

public interface ICorrelationContext
{
    Guid CorrelationId { get; }
    
    Guid MessageId { get; }
    
    Guid CausationId { get; }
}