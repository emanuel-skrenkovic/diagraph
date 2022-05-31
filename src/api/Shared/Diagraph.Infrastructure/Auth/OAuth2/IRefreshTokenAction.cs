namespace Diagraph.Infrastructure.Auth.OAuth2;

public interface IRefreshTokenAction
{
    Task<OAuth2TokenResponse> ExecuteAsync(string refreshToken, params string[] scope);
}