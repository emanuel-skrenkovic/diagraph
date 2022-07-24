using Diagraph.Infrastructure.Integrations.Google;
using Diagraph.Infrastructure.Sessions;
using Microsoft.Net.Http.Headers;

namespace Diagraph.Infrastructure.Notifications.Google;

public class GoogleAuthDelegatingHandler : DelegatingHandler
{
    private readonly GoogleAuthorizer _authorizer;
    private readonly SessionManager   _session;

    public GoogleAuthDelegatingHandler(GoogleAuthorizer authorizer, SessionManager session)
    {
        _authorizer = authorizer;
        _session    = session;
    } 
    
    protected override async Task<HttpResponseMessage> SendAsync
    (
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    )
    {
        Guid userId = await _session.GetAsync<Guid>(SessionConstants.UserId);
        
        string accessToken = await _authorizer.EnsureAuthorizedAsync(userId);
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