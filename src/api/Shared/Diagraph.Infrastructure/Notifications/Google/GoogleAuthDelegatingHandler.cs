using Diagraph.Infrastructure.Integrations.Google;

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
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        return await base.SendAsync(request, cancellationToken);
    }
}