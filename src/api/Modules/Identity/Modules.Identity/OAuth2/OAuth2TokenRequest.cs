using System.Text.Json.Serialization;

namespace Diagraph.Modules.Identity.OAuth2;

public class OAuth2TokenRequest
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }
    
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; }
    
    [JsonPropertyName("code")]
    public string Code { get; set; }
    
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; }
    
    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; }
}