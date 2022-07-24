using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Auth.OAuth2;
using Diagraph.Infrastructure.Dynamic;
using Diagraph.Infrastructure.Integrations.Google.InterModuleIntegration;
using Diagraph.Infrastructure.Sessions;
using MediatR;

namespace Diagraph.Infrastructure.Integrations.Google;

public class GoogleAuthorizer : IIntegrationSession
{
    private readonly IAuthorizationCodeFlow _authFlow;
    private readonly IRefreshTokenAction    _refreshToken;
    private readonly GoogleScopes           _scopes;
    private readonly SessionManager         _sessionManager;
    private readonly IMediator              _mediator;

    public GoogleAuthorizer
    (
        IAuthorizationCodeFlow authFlow,
        IRefreshTokenAction    refreshToken, 
        GoogleScopes           scopes,
        SessionManager         sessionManager,
        IMediator              mediator
    )
    {
        _authFlow       = authFlow;
        _refreshToken   = refreshToken;
        _scopes         = scopes;
        _sessionManager = sessionManager;
        _mediator       = mediator;
    }

    public IAuthorizationCodeFlow AuthFlow => _authFlow;

    public IRefreshTokenAction RefreshToken => _refreshToken;

    public GoogleScopes Scopes => _scopes;
    
    public async Task<string> EnsureAuthorizedAsync(Guid userId)
    {
        TokenData tokenData = await _sessionManager.GetAsync<TokenData>
        (
            GoogleIntegrationConsts.AccessToken
        );

        string   accessToken = tokenData?.AccessToken;
        DateTime issuedAtUtc = tokenData?.IssuedAtUtc ?? DateTime.MinValue;
        long     expiresIn   = tokenData?.ExpiresIn ?? 0;

        if (tokenData is not null && issuedAtUtc.AddSeconds(expiresIn) > DateTime.UtcNow) 
            return accessToken;

        string refreshToken = await _sessionManager.GetAsync<string>
        (
            GoogleIntegrationConsts.RefreshToken
        );
            
        if (refreshToken is null) 
            throw new InvalidOperationException("User is not integrated with Google.");

        OAuth2TokenResponse tokenResponse = await _refreshToken.ExecuteAsync(refreshToken);
        await _sessionManager.SaveAsync
        (
            GoogleIntegrationConsts.AccessToken, 
            new TokenData(tokenResponse.AccessToken, DateTime.UtcNow, tokenResponse.ExpiresIn)
        );
        
        await _mediator.Publish(new AccessTokenRefreshedNotification(userId, tokenResponse.AccessToken));

        return tokenResponse.AccessToken;
    }
    
    public async Task InitializeSessionDataAsync
    (
        string   accessToken, 
        DateTime issuedAt,
        long     expiresIn, 
        string   refreshToken
    )
    {
        await _sessionManager.SaveAsync
        (
            GoogleIntegrationConsts.AccessToken,
            new TokenData(accessToken, issuedAt, expiresIn)
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

        TokenData token = info.AccessToken;
        return InitializeSessionDataAsync
        (
            token.AccessToken,
            token.IssuedAtUtc,
            token.ExpiresIn,
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