using Diagraph.Infrastructure.Cache;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Infrastructure.Api;

public class ResponseContainer
{
    public int StatusCode { get; set; }
}

public class ResponseContainer<TResponse>
    where TResponse : notnull, new()
{
    public int StatusCode { get; set; }
    
    public TResponse Response { get; set; }
}

public class IdempotencyCheck<TRequest, TResponse> : IPreProcessor<TRequest>
    where TRequest : IIdempotentRequest, new() 
    where TResponse : new()
{
    public async Task PreProcessAsync
    (
        TRequest req, 
        HttpContext ctx, 
        List<ValidationFailure> failures, 
        CancellationToken ct
    )
    {
        var cache = ctx.RequestServices.GetRequiredService<ICache>();
        
        ResponseContainer<TResponse> container = cache
            .Get<ResponseContainer<TResponse>, string>(req.IdempotencyKey);
        
        if (container is not null)
        {
            await ctx.Response.SendAsync(container.Response, container.StatusCode, cancellation: ct);
        }
        else
        {
            // While the request is being processed,
            // return HTTP 409 - Conflict.
            cache.Set
            (
                req.IdempotencyKey, 
                new ResponseContainer<TResponse> { StatusCode = 409 }
            );
        }
    }
}

public class IdempotencyCheck<TRequest> : IPreProcessor<TRequest>
    where TRequest : IIdempotentRequest, new()
{
    public async Task PreProcessAsync
    (
        TRequest                req,
        HttpContext             ctx,
        List<ValidationFailure> failures,
        CancellationToken       ct
    )
    {
        var cache = ctx.RequestServices.GetRequiredService<ICache>();

        ResponseContainer container = cache.Get<ResponseContainer, string>(req.IdempotencyKey);

        if (container is not null)
        {
            HttpResponse response = ctx.Response;
            response.StatusCode = container.StatusCode;
            await  response.StartAsync(ct); 
        }
        else
        {
            // While the request is being processed,
            // return HTTP 409 - Conflict.
            cache.Set
            (
                req.IdempotencyKey, 
                new ResponseContainer { StatusCode = 409 }
            );
        }
    }
}

public class IdempotencyUpdate<TRequest, TResponse> : IPostProcessor<TRequest, TResponse>
    where TRequest : IIdempotentRequest, new()
    where TResponse : notnull, new()
{
    public Task PostProcessAsync
    (
        TRequest req, 
        TResponse res, 
        HttpContext ctx, 
        IReadOnlyCollection<ValidationFailure> failures, 
        CancellationToken ct
    )
    {
        var cache = ctx.RequestServices.GetRequiredService<ICache>();
        
        cache.Set
        (
            req.IdempotencyKey, 
            new ResponseContainer<TResponse>
            {
                StatusCode = ctx.Response.StatusCode,
                Response   = res
            }, 
            TimeSpan.FromHours(24)
        );

        return Task.CompletedTask;
    }
}

public class IdempotencyUpdate<TRequest> : IPostProcessor<TRequest, object>
    where TRequest : IIdempotentRequest
{
    public Task PostProcessAsync
    (
        TRequest req, 
        object res, 
        HttpContext ctx, 
        IReadOnlyCollection<ValidationFailure> failures, 
        CancellationToken ct
    )
    {
        var cache = ctx.RequestServices.GetRequiredService<ICache>();
        
        cache.Set
        (
            req.IdempotencyKey, 
            new ResponseContainer { StatusCode = ctx.Response.StatusCode }, 
            TimeSpan.FromHours(24)
        );

        return Task.CompletedTask;
    }
}

public abstract class IdempotentEndpoint<TRequest> : Endpoint<TRequest>
    where TRequest : IIdempotentRequest, new()
{
    public override void Configure()
    {
        PreProcessors(new IdempotencyCheck<TRequest>());
        PostProcessors(new IdempotencyUpdate<TRequest>());
    }
}

public abstract class IdempotentEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : IIdempotentRequest, new()
    where TResponse : notnull, new()
{
    public override void Configure()
    {
        PreProcessors(new IdempotencyCheck<TRequest, TResponse>());
        PostProcessors(new IdempotencyUpdate<TRequest, TResponse>());
    }
}