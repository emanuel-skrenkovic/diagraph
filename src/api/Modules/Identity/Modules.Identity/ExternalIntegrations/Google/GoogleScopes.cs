using System.Collections.Specialized;
using System.Web;
using Diagraph.Modules.Identity.ExternalIntegrations.Google.Configuration;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;

namespace Diagraph.Modules.Identity.ExternalIntegrations.Google;

public class GoogleScopes
{
    private readonly GoogleCredentials   _credentials;
    private readonly GoogleConfiguration _configuration;

    public GoogleScopes(GoogleCredentials credentials, GoogleConfiguration configuration)
    {
        _credentials   = credentials;
        _configuration = configuration;
    }
    
    public async Task<IEnumerable<string>> RequestRequiredAsync(string api, string version) 
    {
        DiscoveryService service = new
        (
            new BaseClientService.Initializer
            {
                ApplicationName = _credentials.ApplicationName,
                ApiKey          = _credentials.ApiKey
            }
        );

        RestDescription description = await service
            .Apis
            .GetRest(api, version)
            .ExecuteAsync();
        
        return description.Auth.Oauth2.Scopes.Keys;
    }

    public string GenerateRequestsScopesUrl(IEnumerable<string> scopes, string redirectUri)
    {
        UriBuilder builder         = new(_configuration.AuthUrl);
        
        NameValueCollection query = HttpUtility.ParseQueryString("");
        query.Add(OAuth2Constants.Scope, scopes.Aggregate((acc, val) => $"{acc} {val}"));
        query.Add(OAuth2Constants.ClientId, _configuration.ClientId);
        query.Add(OAuth2Constants.RedirectUri, redirectUri);
        query.Add(OAuth2Constants.ResponseType, OAuth2Constants.ResponseTypes.Code);
        query.Add("access_type", "offline");
        
        builder.Query = query.ToString()!;
        
        return builder.ToString();
    }
}