using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Diagraph.Infrastructure.Tests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static T CreatedId<T>(this HttpResponseMessage response)
    {
        return response.Headers.Location.AsId<T>();
    }

    public static string GetCookie(this HttpResponseMessage response, string cookieName, string uriString)
    {
        Uri uri = new(uriString);
        CookieContainer cookies = new CookieContainer();
        foreach (var cookieHeader in response.Headers.GetValues("Set-Cookie"))
            cookies.SetCookies(uri, cookieHeader);
         
        return cookies.GetCookies(uri).FirstOrDefault(c => c.Name == cookieName)?.Value;       
    }
}