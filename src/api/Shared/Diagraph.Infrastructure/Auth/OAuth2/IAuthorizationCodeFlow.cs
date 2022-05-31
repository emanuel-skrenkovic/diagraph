namespace Diagraph.Infrastructure.Auth.OAuth2;

public interface IAuthorizationCodeFlow
{
    Task<OAuth2TokenResponse> ExecuteAsync(string code, string redirectUri); // TODO: needs state
}