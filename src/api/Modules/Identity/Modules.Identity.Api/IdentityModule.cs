using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Modules;
using Diagraph.Infrastructure.Modules.Extensions;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations.Google;
using Diagraph.Modules.Identity.Registration;
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
        
        services.AddSingleton(new EmailServerConfiguration
        {
            From = configuration.GetValue<string>("Mailhog:From"),
            Host = configuration["Mailhog:Host"],
            Port = configuration.GetValue<int>("Mailhog:Port")
        });
        services.AddScoped<IEmailClient, EmailClient>();

        services.AddSingleton(new GoogleConfiguration
        {
            AuthUrl      = configuration["Google:AuthUrl"],
            RedirectUrl  = configuration["Google:RedirectUrl"],
            ClientId     = configuration["Google:ClientId"],
            ClientSecret = configuration["Google:ClientSecret"]
        });

        services.AddScoped<PasswordTool>();
        services.AddScoped<IHashTool, Sha256HashTool>();
        services.AddScoped<UserConfirmation>();
        
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