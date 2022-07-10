using System;
using Diagraph.Infrastructure.EventSourcing.Contracts;

namespace Diagraph.Infrastructure.Tests;

public class MockCorrelationContext : ICorrelationContext
{
    public Guid CorrelationId { get; set; }
    public Guid MessageId     { get; set; }
    public Guid CausationId   { get; set; }
}