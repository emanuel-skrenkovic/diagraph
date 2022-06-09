namespace Diagraph.Infrastructure.Api;

public interface IIdempotentRequest
{
    string IdempotencyKey { get; }
}