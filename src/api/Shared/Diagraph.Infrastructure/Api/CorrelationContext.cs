using Diagraph.Infrastructure.EventSourcing.Contracts;

namespace Diagraph.Infrastructure.Api;

public class CorrelationContext : ICorrelationContext
{
    public Guid CorrelationId { get; set; }
    public Guid MessageId { get; set; }
    public Guid CausationId { get; set; }
}