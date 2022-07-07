namespace Diagraph.Infrastructure.Auth;

public record TokenData(string AccessToken, DateTime IssuedAtUtc, long ExpiresIn);