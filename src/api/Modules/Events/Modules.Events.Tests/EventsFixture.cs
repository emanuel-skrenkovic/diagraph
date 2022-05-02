using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Tests;
using Diagraph.Infrastructure.Tests.Docker;
using Diagraph.Modules.Events.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Diagraph.Modules.Events.Tests;

public class EventsFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    
    internal readonly PostgreSqlContainer<EventsDbContext> Postgres;

    public HttpClient Client => _webApplicationFactory.CreateClient();

    public EventsFixture()
    {
        IConfiguration configuration = new ConfigurationManager()
            .AddJsonFile("module.events.integration-test.json")
            .Build();
        
        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration
                (
                    configurationBuilder => configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                );
                
                builder.UseEnvironment("integration-test");
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthScheme;
                        options.DefaultChallengeScheme    = TestAuthenticationHandler.AuthScheme;
                    }).AddTestAuth(_ => { });
                });
            });
        
        Postgres = new PostgreSqlContainer<EventsDbContext>
        (
            configuration["DatabaseConfiguration:ConnectionString"]
        );
    }
    
    public async Task ExecuteAsync<TService>(Func<TService, Task> action)
    {
        using IServiceScope scope = _webApplicationFactory.Services.CreateScope();
        
        await action
        (
            scope.ServiceProvider.GetRequiredService<TService>()
        );
    }

    public Task InitializeAsync() => Postgres.InitializeAsync();

    public Task DisposeAsync() => Postgres.DisposeAsync();
}