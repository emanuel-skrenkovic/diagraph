using System.Net.Http.Json;
using Diagraph.Modules.Identity.OAuth2;

namespace Diagraph.Modules.Identity.ExternalIntegrations.Google;

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
        HttpClient client = _clientFactory.CreateClient("GoogleOAuth2"); // TODO: constant
        
        HttpResponseMessage response = await client.PostAsJsonAsync
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