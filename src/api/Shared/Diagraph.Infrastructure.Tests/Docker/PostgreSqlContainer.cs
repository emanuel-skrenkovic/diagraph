using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Infrastructure.Tests.Docker;

public class PostgreSqlContainer<TContext> : IAsyncLifetime where TContext : DbContext
{
    private readonly DockerContainer _container;
    private readonly Func<TContext> _contextFactory;
    
    #region DockerParameters

    private const string ContainerName = "integration-tests.postgres";
    private const string ImageName = "postgres:14-alpine";

    private readonly List<string> _env = new()
    {
        "POSTGRES_USER=postgres",
        "POSTGRES_PASSWORD=postgres"
    };

    #endregion
    
    public TContext Context { get; private set; }

    public PostgreSqlContainer(string connectionString, Func<TContext> contextFactory)
    {
        string portKv = connectionString
            .Split(';')
            .First(kv => kv.StartsWith("Port="));
        string port = portKv.Substring(5, portKv.Length - 5);

        _container = new(ContainerName, ImageName, _env, new() { ["5432"] = port });
        _container.CheckStatus = CheckConnectionAsync;

        _contextFactory = contextFactory;
    }

    public async Task SeedAsync(Func<TContext, Task> seedAction)
    {
        await EnsureMigratedAsync();
        await seedAction(Context);
    }

    public async Task CleanAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await EnsureMigratedAsync();
    }

    private Task EnsureMigratedAsync() => Context.Database.MigrateAsync();

    private async Task<bool> CheckConnectionAsync()
    {
        try
        {
            Context = _contextFactory();
            if (Context == null) return false;

            await EnsureMigratedAsync();

            return true;
        }
        catch
        {
            return false; 
        }
    }
    
    #region IAsyncLifetime

    public async Task InitializeAsync()
    {
        await _container.InitializeAsync();
        await EnsureMigratedAsync();
    }

    public async Task DisposeAsync()
    {
        if (Context != null) await Context.DisposeAsync();
        await _container.DisposeAsync();
    }
    
    #endregion
}