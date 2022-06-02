using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Infrastructure.Integrations.Google;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations;
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
    public async Task Creates_New_Integration(ConfirmGoogleTasksScopesCommand command)
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
            info.GrantedScopes.Should().IntersectWith(command.Scope);
            info.RefreshToken.Should().NotBeNullOrWhiteSpace();
        });
        
        await _fixture.Postgres.CleanAsync();
    }
    
    [Theory, CustomizedAutoData]
    public async Task Updates_Existing_Integration
    (
        ConfirmGoogleTasksScopesCommand command, 
        ConfirmGoogleTasksScopesCommand command2
    )
    {
        // Arrange
        await RegisterUser();
        
        await _fixture.Client.PutAsJsonAsync
        (
            "auth/external-access/google/scopes/confirm",
            command
        );

        command2.Scope = command2.Scope.Concat(command.Scope).ToArray();
        
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
            info.GrantedScopes.Should().IntersectWith(command.Scope);
            info.GrantedScopes.Should().IntersectWith(command2.Scope);
            info.RefreshToken.Should().NotBeNullOrWhiteSpace();
        });

        await _fixture.Postgres.CleanAsync();
    }

    private async Task RegisterUser()
    {
        await _fixture.ExecuteAsync<IdentityDbContext>(async context =>
        {
            if (await context.Users.AnyAsync(u => u.Id == IdentityFixture.RegisteredUserId)) return;
            
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