using System;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Parsing.Templates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Infrastructure.Tests.Fixture;

public class DiagraphFixture
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    internal Templates TemplatesHelper { get; }

    public DiagraphFixture()
    {
        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseConfiguration
                (
                    new ConfigurationManager()
                    .AddJsonFile("Fixture/tests.diagraph.json")
                    .Build()
                );
                builder.ConfigureTestServices(services =>
                {
                    services
                        .AddAuthentication(TestAuthenticationHandler.AuthScheme)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>
                        (
                            TestAuthenticationHandler.AuthScheme,
                            _ => { }
                        );
                });
            });

        TemplatesHelper = new Templates(this);
    }

    public async Task<TResponse> PostAsJsonAsync<TRequest, TResponse>(string path, TRequest request)
    { 
        var client = _webApplicationFactory
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Integration-Test");
        
        HttpResponseMessage response = await client.PostAsJsonAsync(path, request);

        response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<TResponse>
        (
            await response.Content.ReadAsStringAsync()
        );
    }
    
    public async Task PostAsJsonAsync<TRequest>(string path, TRequest request)
    {
        HttpResponseMessage response = await _webApplicationFactory
            .CreateClient()
            .PostAsJsonAsync(path, request);

        response.EnsureSuccessStatusCode();
    }

    public TResponse Execute<TService, TResponse>(Func<TService, TResponse> action)
        => action
        (
            _webApplicationFactory.Services.GetRequiredService<TService>()
        );
    
    
    internal class Templates
    {
        private readonly DiagraphFixture _fixture;

        internal Templates(DiagraphFixture fixture)
            => _fixture = fixture;
        
        public ValueTask<ImportTemplate> Get(int id) 
            => _fixture.Execute<DiagraphDbContext, ValueTask<ImportTemplate>>
            (
                context => context.Templates.FindAsync(id)
            );
    }
}