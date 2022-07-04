using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Auth.OAuth2;
using Diagraph.Infrastructure.Dynamic;
using Diagraph.Infrastructure.Sessions;

namespace Diagraph.Infrastructure.Integrations.Google;

public class GoogleAuthorizer : IIntegrationSession
{
    private readonly IAuthorizationCodeFlow _authFlow;
    private readonly IRefreshTokenAction    _refreshToken;
    private readonly GoogleScopes           _scopes;
    private readonly SessionManager         _sessionManager;

    public GoogleAuthorizer
    (
        IAuthorizationCodeFlow authFlow,
        IRefreshTokenAction    refreshToken, 
        GoogleScopes           scopes,
        SessionManager         sessionManager
    )
    {
        _authFlow       = authFlow;
        _refreshToken   = refreshToken;
        _scopes         = scopes;
        _sessionManager = sessionManager;
    }

    public IAuthorizationCodeFlow AuthFlow => _authFlow;

    public IRefreshTokenAction RefreshToken => _refreshToken;

    public GoogleScopes Scopes => _scopes;
    
    public async Task<string> EnsureAuthorizedAsync()
    {
        TokenData tokenData = await _sessionManager
            .GetAsync<TokenData>(GoogleIntegrationConsts.AccessToken);

        string accessToken = tokenData?.AccessToken;
        long expiresIn     = tokenData?.ExpiresIn ?? 0;

        if (accessToken is not null && DateTime.UtcNow.AddSeconds(expiresIn) > DateTime.UtcNow) 
            return accessToken;

        string refreshToken = await _sessionManager
            .GetAsync<string>(GoogleIntegrationConsts.RefreshToken);
            
        if (refreshToken is null) 
            throw new InvalidOperationException("User is not integrated with Google.");

        OAuth2TokenResponse tokenResponse = await _refreshToken.ExecuteAsync(refreshToken);
        await _sessionManager.SaveAsync
        (
            GoogleIntegrationConsts.AccessToken, 
            new TokenData(tokenResponse.AccessToken, tokenResponse.ExpiresIn)
        );
        
        // TODO: doesn't save or read the current access token from the database.

        return tokenResponse.AccessToken;
    }
    
    public async Task InitializeSessionDataAsync
    (
        string accessToken, 
        long   expiresIn, 
        string refreshToken
    )
    {
        await _sessionManager.SaveAsync
        (
            GoogleIntegrationConsts.AccessToken,
            new TokenData(accessToken, expiresIn)
        );
        await _sessionManager.SaveAsync
        (
            GoogleIntegrationConsts.RefreshToken,
            refreshToken
        ); 
    }
    
    #region IIntegrationSession

    public ExternalProvider Provider => ExternalProvider.Google;

    public Task InitializeAsync(IDynamicDataContainer dataContainer)
    {
        var info = dataContainer.GetData<GoogleIntegrationInfo>();

        return InitializeSessionDataAsync
        (
            info.AccessToken.AccessToken,
            info.AccessToken.ExpiresIn,
            info.RefreshToken
        );
    }

    public async Task TerminateAsync()
    {
        await _sessionManager.RemoveAsync(GoogleIntegrationConsts.AccessToken);
        await _sessionManager.RemoveAsync(GoogleIntegrationConsts.RefreshToken);
    }
    
    #endregion
}