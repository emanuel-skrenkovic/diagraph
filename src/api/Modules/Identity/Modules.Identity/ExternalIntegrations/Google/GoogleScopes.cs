using Diagraph.Modules.Identity.ExternalIntegrations.Google.Configuration;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;

namespace Diagraph.Modules.Identity.ExternalIntegrations.Google;

public class GoogleScopes
{
    private readonly GoogleCredentials _credentials;

    public GoogleScopes(GoogleCredentials credentials)
        => _credentials = credentials;
    
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
}