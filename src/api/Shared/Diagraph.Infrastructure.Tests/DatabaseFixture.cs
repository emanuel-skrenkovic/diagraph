using System;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Tests.Docker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Diagraph.Infrastructure.Tests;

public class DatabaseFixture<TContext> : IAsyncLifetime where TContext : DbContext
{
     private readonly PostgreSqlContainer<TContext>  _container;
 
     public DatabaseFixture(string moduleName, Func<TContext> contextFactory)
     {
         IConfiguration configuration = new ConfigurationManager()
             .AddJsonFile($"module.{moduleName}.integration-test.json")
             .Build();
         
         _container = new PostgreSqlContainer<TContext>
         (
             configuration
                 .GetSection(DatabaseConfiguration.SectionName)
                 .Get<DatabaseConfiguration>()
                 .ConnectionString,
             contextFactory
         );
     }
     
     public Task CleanAsync() => _container.CleanAsync();
     
     #region IAsyncLifetime
 
     public Task InitializeAsync() => _container.InitializeAsync();
 
     public Task DisposeAsync() => _container.DisposeAsync();   
     
     #endregion
}