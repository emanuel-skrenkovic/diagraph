using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations;
using Diagraph.Modules.Identity.ExternalIntegrations.Google;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Modules.Identity.Tests.Integration;

[Collection(nameof(IdentityFixtureCollection))]
public class GoogleIntegrationsTests
{
    private readonly IdentityFixture _fixture;

    public GoogleIntegrationsTests(IdentityFixture fixture) 
        => _fixture = fixture;

    [Theory, CustomizedAutoData]
    public async Task Returns_Scopes_Request_Google_Url(RequestGoogleScopesAccessCommand command)
    {
        // Act
        HttpResponseMessage response = await _fixture.Client.PostAsJsonAsync
        (
            "auth/external-access/google/scopes/request",
            command
        );

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = JsonSerializer.Deserialize<RequestGoogleScopesAccessResponse>
        (
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        responseBody.Should().NotBeNull();
        responseBody!.RedirectUri.Should().NotBeNullOrWhiteSpace();
    }

    [Theory, CustomizedAutoData]
    public async Task Creates_New_Integration(ConfirmGoogleScopesAccessCommand command)
    {
        // Arrange
        await RegisterUser();
        
        // Act
        HttpResponseMessage response = await _fixture.Client.PutAsJsonAsync
        (
            "auth/external-access/google/scopes/confirm",
            command
        ); 
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.ExecuteAsync<IdentityDbContext>(async context =>
        {
            (await context.UserExternalIntegrations.AnyAsync()).Should().BeTrue();
            
            External external = await context.UserExternalIntegrations.FirstOrDefaultAsync();
            external.Should().NotBeNull();

            var info = external!.GetData<GoogleIntegrationInfo>();
            info.Should().NotBeNull();
            info.GrantedScopes.Should().IntersectWith(command.Scopes);
        });
        
        await _fixture.Postgres.CleanAsync();
    }
    
    [Theory, CustomizedAutoData]
    public async Task Updates_Existing_Integration
    (
        ConfirmGoogleScopesAccessCommand command, 
        ConfirmGoogleScopesAccessCommand command2
    )
    {
        // Arrange
        await RegisterUser();
        
        await _fixture.Client.PutAsJsonAsync
        (
            "auth/external-access/google/scopes/confirm",
            command
        );

        command2.Scopes = command2.Scopes.Concat(command.Scopes).ToArray();
        
        // Act
        HttpResponseMessage response = await _fixture.Client.PutAsJsonAsync
        (
            "auth/external-access/google/scopes/confirm",
            command
        );       
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.ExecuteAsync<IdentityDbContext>(async context =>
        {
            (await context.UserExternalIntegrations.AnyAsync()).Should().BeTrue();
            
            External external = await context.UserExternalIntegrations.FirstOrDefaultAsync();
            external.Should().NotBeNull();

            var info = external!.GetData<GoogleIntegrationInfo>();
            info.Should().NotBeNull();
            info.GrantedScopes.Should().IntersectWith(command.Scopes);
            info.GrantedScopes.Should().IntersectWith(command2.Scopes);
        });

        await _fixture.Postgres.CleanAsync();
    }

    private async Task RegisterUser()
    {
        await _fixture.ExecuteAsync<IdentityDbContext>(async context =>
        {
            context.Users.Add
            (
                new User
                {
                    Id           = IdentityFixture.RegisteredUserId,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                    Email        = "",
                    PasswordHash = ""
                }
            );
            await context.SaveChangesAsync();
        });        
    }
}