using System.Collections.Specialized;
using System.Web;
using Diagraph.Infrastructure.Auth.OAuth2;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;

namespace Diagraph.Infrastructure.Integrations.Google;

public class GoogleScopes
{
    private readonly GoogleConfiguration _configuration;

    public GoogleScopes(GoogleConfiguration configuration) => _configuration = configuration;
    
    public async Task<IEnumerable<string>> RequestRequiredAsync(string api, string version) 
    {
        DiscoveryService service = new(new BaseClientService.Initializer());

        RestDescription description = await service
            .Apis
            .GetRest(api, version)
            .ExecuteAsync();
        
        return description.Auth.Oauth2.Scopes.Keys;
    }

    public string GenerateRequestsScopesUrl
    (
        IEnumerable<string> scopes, 
        string redirectUri, 
        string state = null
    )
    {
        UriBuilder builder = new(_configuration.AuthUrl);
        
        NameValueCollection query = HttpUtility.ParseQueryString("");
        query.Add(OAuth2Constants.Scope, scopes.Aggregate((acc, val) => $"{acc} {val}"));
        query.Add(OAuth2Constants.ClientId, _configuration.ClientId);
        query.Add(OAuth2Constants.RedirectUri, redirectUri);
        query.Add(OAuth2Constants.ResponseType, OAuth2Constants.ResponseTypes.Code);
        query.Add(GoogleOAuth2Constants.AccessType, GoogleOAuth2Constants.AccessTypes.Offline);

        if (state is not null)
        {
            query.Add(OAuth2Constants.State, state);
        }
        
        builder.Query = query.ToString()!;
        
        return builder.ToString();
    }
}