using Diagraph.Infrastructure.Auth.OAuth2;
using Diagraph.Infrastructure.Integrations.Google;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;
using FastEndpoints;

namespace Diagraph.Modules.Identity.Api.ExternalIntegrations.Google;

public class RequestGoogleTasksScopesEndpoint : EndpointWithoutRequest
{
    private readonly GoogleAuthorizer _authorizer;

    public RequestGoogleTasksScopesEndpoint(GoogleAuthorizer authorizer)
        => _authorizer = authorizer;
    
    public override void Configure()
        => Get("auth/external-access/google/tasks/scopes/required");

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendOkAsync
        (
            new RequestGoogleTasksScopesAccessResult
            {
                RedirectUri = _authorizer.Scopes.GenerateRequestsScopesUrl
                (
                    await _authorizer.Scopes.RequestRequiredAsync("tasks", "v1"),
                    Query<string>("redirectUri"),
                    Query<string>(OAuth2Constants.State, isRequired: false)
                )
            },
            ct
        );
    }
}