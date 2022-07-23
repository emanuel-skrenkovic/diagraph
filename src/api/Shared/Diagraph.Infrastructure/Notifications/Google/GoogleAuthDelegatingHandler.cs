using Diagraph.Infrastructure.Integrations.Google;
using Microsoft.Net.Http.Headers;

namespace Diagraph.Infrastructure.Notifications.Google;

public class GoogleAuthDelegatingHandler : DelegatingHandler
{
    private readonly GoogleAuthorizer _authorizer;

    public GoogleAuthDelegatingHandler(GoogleAuthorizer authorizer)
        => _authorizer = authorizer;
    
    protected override async Task<HttpResponseMessage> SendAsync
    (
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    )
    {
        string accessToken = await _authorizer.EnsureAuthorizedAsync();
        string headerName  = HeaderNames.Authorization;

        // Seems that there is no way to replace the value of a header without
        // removing it and adding a new value with the same key.
        if (request.Headers.Contains(headerName)) 
        {
            request.Headers.Remove(headerName);
        }
        
        request.Headers.Add(headerName, $"Bearer {accessToken}");
        
        return await base.SendAsync(request, cancellationToken);
    }
}