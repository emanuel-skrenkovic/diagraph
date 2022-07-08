using Diagraph.Infrastructure.Auth.OAuth2;
using Diagraph.Infrastructure.Integrations.Google;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Contracts;
using FastEndpoints;

namespace Diagraph.Modules.Identity.Api.ExternalIntegrations.Google;

public class RequestGoogleScopesEndpoint : EndpointWithoutRequest
{
    private readonly GoogleAuthorizer _authorizer;

    public RequestGoogleScopesEndpoint(GoogleAuthorizer authorizer)
        => _authorizer = authorizer;
    
    public override void Configure() => Get("auth/external-access/google/scopes/required");

    public override async Task HandleAsync(CancellationToken ct)
    {
        GoogleScopes discovery = _authorizer.Scopes;
            
        IEnumerable<string> tasksApiScopes   = await discovery.RequestRequiredAsync("tasks", "v1");
        IEnumerable<string> fitnessApiScopes = await discovery.RequestRequiredAsync("fitness", "v1");

        IEnumerable<string> scopes = tasksApiScopes.Concat(fitnessApiScopes);
        
        await SendOkAsync
        (
            new RequestGoogleTasksScopesAccessResult
            {
                RedirectUri = _authorizer.Scopes.GenerateRequestsScopesUrl
                (
                    scopes,
                    Query<string>("redirectUri"),
                    Query<string>(OAuth2Constants.State, isRequired: false)
                )
            },
            ct
        );
    }
}