namespace Diagraph.Infrastructure.Auth.OAuth2;

public class OAuth2Constants
{
    public const string GrantType    = "grant_type";
    public const string Scope        = "scope";
    public const string ClientId     = "client_id";
    public const string ClientSecret = "client_secret";
    public const string RedirectUri  = "redirect_uri";
    public const string ResponseType = "response_type";

    public class GrantTypes
    {
        public const string AuthorizationCode = "authorization_code";
        public const string RefreshToken      = "refresh_token";
    }

    public class ResponseTypes
    {
        public const string Code = "code";
    }
}