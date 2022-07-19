using Diagraph.Infrastructure.EventSourcing.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Diagraph.Infrastructure.Api;

public static class CorrelationIdMiddleware
{
    public static Task Handle(HttpContext context, Func<Task> next)
    {
        CorrelationContext correlationContext = context
            .RequestServices
            .GetRequiredService<ICorrelationContext>() as CorrelationContext;
        if (correlationContext is null) return next();

        Guid correlationId = context.Request.Headers.TryGetValue
        (
            "correlation-id", 
            out StringValues correlationIdHeader
        ) ? Guid.Parse(correlationIdHeader) : Guid.NewGuid();

        correlationContext.CorrelationId = correlationId;
        correlationContext.CausationId   = correlationId;
        correlationContext.MessageId     = correlationId;

        return next();   
    }
}