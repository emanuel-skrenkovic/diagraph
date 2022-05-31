using System.Net.Http.Json;
using Diagraph.Infrastructure.Auth.OAuth2;

namespace Diagraph.Infrastructure.Integrations.Google;

public class GoogleAuthorizationCodeFlow : IAuthorizationCodeFlow
{
    private readonly GoogleConfiguration _configuration;
    private readonly IHttpClientFactory _clientFactory;

    public GoogleAuthorizationCodeFlow
    (
        GoogleConfiguration configuration, 
        IHttpClientFactory clientFactory
    )
    {
        _configuration = configuration;
        _clientFactory = clientFactory;
    }
    
    public async Task<OAuth2TokenResponse> ExecuteAsync(string code, string redirectUri)
    {
        Ensure.NotNullOrEmpty(code);
        Ensure.NotNullOrEmpty(redirectUri);
        
        HttpResponseMessage response = await _clientFactory
            .CreateClient(GoogleIntegrationConsts.OAuthClientName)
            .PostAsJsonAsync
            (
                "/token",
                new OAuth2TokenRequest
                {
                    ClientId     = _configuration.ClientId,
                    ClientSecret = _configuration.ClientSecret,
                    Code         = code,
                    GrantType    = OAuth2Constants.GrantTypes.AuthorizationCode,
                    RedirectUri  = redirectUri
                }
            );
        response.EnsureSuccessStatusCode(); // TODO: proper error handling

        return await response.Content.ReadFromJsonAsync<OAuth2TokenResponse>();
    }
}