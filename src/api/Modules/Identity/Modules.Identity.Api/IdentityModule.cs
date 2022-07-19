using Diagraph.Infrastructure.Api;
using Diagraph.Infrastructure.Cache.Redis;
using Diagraph.Infrastructure.Cache.Redis.Extensions;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.EventSourcing.EventStore;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Integrations.Extensions;
using Diagraph.Infrastructure.Modules;
using Diagraph.Infrastructure.Modules.Extensions;
using Diagraph.Infrastructure.ProcessManager.Contracts;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.Registration;
using Diagraph.Modules.Identity.UserData;
using EventStore.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Modules.Identity.Api;

public class IdentityModule : Module
{
    const string LoginPath  = "/auth/login";
    const string LogoutPath = "/auth/logout";
    
    public override string ModuleName => "identity";

    protected override void RegisterServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddAutoMapper(typeof(IdentityDbContext).Assembly);
        
        services.AddPostgres<IdentityDbContext>
        (
            configuration
                .GetSection(DatabaseConfiguration.SectionName)
                .Get<DatabaseConfiguration>()
        );

        services.AddRedis
        (
            configuration
                .GetSection(RedisConfiguration.SectionName)
                .Get<RedisConfiguration>()
        );
        
        services.AddSingleton(new EmailServerConfiguration
        {
            From = configuration.GetValue<string>("Mailhog:From"),
            Host = configuration["Mailhog:Host"],
            Port = configuration.GetValue<int>("Mailhog:Port")
        });
        services.AddScoped<IEmailClient, EmailClient>();

        services.AddScoped<PasswordTool>();
        services.AddScoped<IHashTool, Sha256HashTool>();
        services.AddScoped<UserConfirmation>();

        services.AddGoogleIntegration(configuration);

        services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
        services.AddScoped<ICorrelationContext, CorrelationContext>();
        services.AddScoped<IProcessManager<UserDataRemovalState>, UserDataDeletionProcessManager>();
        services.AddTransient<EventSubscriber>();
        services.AddSingleton
        (
            _ => new EventStoreClient
            (
                EventStoreClientSettings.Create
                (
                    configuration
                        .GetSection(EventStoreConfiguration.SectionName)
                        .Get<EventStoreConfiguration>()
                        .ConnectionString
                )
            )
        );

        services.AddAuthentication
        (
            opts =>
            {
                opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme    = CookieAuthenticationDefaults.AuthenticationScheme;
                opts.DefaultScheme             = CookieAuthenticationDefaults.AuthenticationScheme;
                opts.DefaultSignOutScheme      = CookieAuthenticationDefaults.AuthenticationScheme;
            }
        ).AddCookie
        (
            opts =>
            {
                opts.Cookie.HttpOnly    = true;
                opts.Cookie.IsEssential = true;
                opts.Cookie.SameSite    = SameSiteMode.None;

                opts.LoginPath  = LoginPath;
                opts.LogoutPath = LogoutPath;
            }
        );
    }
}