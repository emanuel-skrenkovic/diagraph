using Diagraph.Infrastructure.Auth.OAuth2;
using Diagraph.Infrastructure.Integrations.Google;
using Diagraph.Infrastructure.Notifications;
using Diagraph.Infrastructure.Notifications.Google;
using Diagraph.Infrastructure.Sessions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Diagraph.Infrastructure.Integrations.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleIntegration
    (
        this IServiceCollection services, 
        IConfiguration configuration
    )
    {
        services.TryAddTransient
        (
            _ => new GoogleConfiguration
            {
                AuthUrl      = configuration["Google:AuthUrl"],
                ClientId     = Environment.GetEnvironmentVariable("DIAGRAPH_GOOGLE_CLIENT_ID"),
                ClientSecret = Environment.GetEnvironmentVariable("DIAGRAPH_GOOGLE_CLIENT_SECRET")
            }
        );
        services.AddHttpClient
        (
            GoogleIntegrationConsts.OAuthClientName,
            c => c.BaseAddress = new(configuration["Google:TokenUrl"])
        );
        services.AddHttpClient
        (
            GoogleIntegrationConsts.AuthenticatedClientName,
            c => c.BaseAddress = new("https://tasks.googleapis.com")
        ).AddHttpMessageHandler<GoogleAuthDelegatingHandler>();
        services.AddTransient<GoogleAuthDelegatingHandler>();
        
        services.TryAddScoped<GoogleAuthorizer>();
        services.TryAddScoped<IIntegrationSession, GoogleAuthorizer>();
        services.TryAddScoped<GoogleScopes>();
        services.TryAddScoped<IRefreshTokenAction, GoogleRefreshTokenAction>();
        services.TryAddScoped<IAuthorizationCodeFlow, GoogleAuthorizationCodeFlow>();
        
        services.TryAddScoped<INotificationScheduler, GoogleTasksScheduler>();
        services.TryAddScoped<SessionManager>();
        
        services.TryAddScoped<IIntegrationSessionProvider, IntegrationSessionProvider>();

        return services;
    }
}