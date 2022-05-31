using System.Net.Http.Json;
using Diagraph.Infrastructure.Auth.OAuth2;

namespace Diagraph.Infrastructure.Integrations.Google;

public class GoogleRefreshTokenAction : IRefreshTokenAction
{
    private readonly GoogleConfiguration _configuration;
    private readonly IHttpClientFactory  _clientFactory;

    public GoogleRefreshTokenAction
    (
        GoogleConfiguration configuration,
        IHttpClientFactory clientFactory
    )
    {
        _configuration = configuration;
        _clientFactory = clientFactory;
    }
    
    public async Task<OAuth2TokenResponse> ExecuteAsync(string refreshToken, params string[] scope)
    {
        Ensure.NotNullOrEmpty(refreshToken);
        
        HttpResponseMessage response = await _clientFactory
            .CreateClient(GoogleIntegrationConsts.OAuthClientName)
            .PostAsJsonAsync
            (
                "/token",
                new OAuth2TokenRequest
                {
                    GrantType    = OAuth2Constants.GrantTypes.RefreshToken,
                    ClientId     = _configuration.ClientId,
                    ClientSecret = _configuration.ClientSecret,
                    RefreshToken = refreshToken,
                    Scope        = scope?.Any() == true 
                        ? scope.Aggregate((agg, s) => $"{agg} {s}")
                        : null
                }
            );
        response.EnsureSuccessStatusCode(); // TODO: proper error handling

        return await response.Content.ReadFromJsonAsync<OAuth2TokenResponse>();
    }
}