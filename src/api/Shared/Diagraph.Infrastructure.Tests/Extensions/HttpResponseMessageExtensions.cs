using System.Net.Http;

namespace Diagraph.Infrastructure.Tests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static T CreatedId<T>(this HttpResponseMessage response)
    {
        return response.Headers.Location.AsId<T>();
    }
}