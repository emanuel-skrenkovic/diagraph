using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Diagraph.Infrastructure.Api.Extensions;

public static class EndpointExtensions
{
    public static Task SendWithoutContentAsync<TRequest, TResponse>
    (
        this Endpoint<TRequest, TResponse> endpoint,
        int                                statusCode,
        CancellationToken                  cancellation = default
    ) where TRequest : notnull, new() where TResponse : notnull, new()
    {
        HttpResponse response = endpoint.HttpContext.Response;
        response.StatusCode = statusCode;
        return response.StartAsync(cancellation);  
    }
    
    public static Task SendCreated<TRequest, TResponse>
    (
        this Endpoint<TRequest, TResponse> endpoint, 
        CancellationToken cancellation = default
    ) where TRequest : notnull, new() where TResponse : notnull, new()
    {
        HttpResponse response = endpoint.HttpContext.Response;
        response.StatusCode = 201;
        return response.StartAsync(cancellation); 
    }

    public static string CorrelationId<TRequest, TResponse>(this Endpoint<TRequest, TResponse> endpoint)
        where TRequest : notnull, new() where TResponse : notnull, new()
        => endpoint.HttpContext.Request.Headers["correlation-id"];
}