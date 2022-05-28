namespace Diagraph.Modules.Identity.ExternalIntegrations.Google;

public class GoogleIntegrationInfo
{
    // TODO: default here?
    public string AuthUrl { get; set; } = "https://accounts.google.com/o/oauth2/v2/auth";
    
    public string RedirectUri { get; set; }

    public string[] GrantedScopes { get; set; }
    
    public string RefreshToken { get; set; } // TODO: insecure
}