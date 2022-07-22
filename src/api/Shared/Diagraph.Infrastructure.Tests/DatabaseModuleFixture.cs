using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Tests.Docker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Diagraph.Infrastructure.Tests;

public class DatabaseModuleFixture<TContext> : IAsyncLifetime where TContext : DbContext
{
     private readonly WebApplicationFactory<Program> _webApplicationFactory;
     
     public readonly PostgreSqlContainer<TContext> Postgres;
 
     public HttpClient Client => _webApplicationFactory.CreateClient();

     public T Service<T>() => _webApplicationFactory.Services.GetRequiredService<T>();
     
     public T ServiceOfType<T, TBase>() where T : TBase
     {
         IEnumerable<TBase> services = _webApplicationFactory
             .Services
             .GetRequiredService<IEnumerable<TBase>>();

         return (T) services.First(s => s.GetType().IsAssignableTo(typeof(T)));
     }
 
     public DatabaseModuleFixture(string moduleName, Action<IServiceCollection> configureServices = null)
     {
         IConfiguration configuration = new ConfigurationManager()
             .AddJsonFile($"module.{moduleName}.integration-test.json")
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
                     
                     configureServices?.Invoke(services);
                 });
             });

         
         Postgres = new PostgreSqlContainer<TContext>
         (
             configuration["DatabaseConfiguration:ConnectionString"],
             () => _webApplicationFactory.Services.GetRequiredService<TContext>()
         );
     }
     
     public async Task ExecuteAsync<TService>(Func<TService, Task> action)
     {
         using IServiceScope scope = _webApplicationFactory.Services.CreateScope();
         
         await action(scope.ServiceProvider.GetRequiredService<TService>());
     }
 
     public Task InitializeAsync() => Postgres.InitializeAsync();
 
     public Task DisposeAsync() => Postgres.DisposeAsync();   
}