using FastEndpoints;

namespace Diagraph.Infrastructure.Api;

public class PlainTextRequest : IPlainTextRequest
{
    public string Content { get; set; }
}