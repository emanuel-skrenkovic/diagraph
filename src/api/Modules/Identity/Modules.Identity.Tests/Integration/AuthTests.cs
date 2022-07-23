using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Infrastructure.Tests.Extensions;
using Diagraph.Modules.Identity.Api.Auth.Contracts;
using Diagraph.Modules.Identity.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Modules.Identity.Tests.Integration;

[Collection(nameof(IdentityFixtureCollection))]
public class AuthTests
{
    private readonly IdentityFixture _fixture;

    public AuthTests(IdentityFixture fixture)
        => _fixture = fixture;
    
    [Theory, CustomizedAutoData]
    public async Task Registers_User(UserRegisterCommand command)
    {
        // Act
        HttpResponseMessage response = await _fixture.Module.Client.PostAsJsonAsync("auth/register", command);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
            user.Should().NotBeNull();

            user!.EmailConfirmed.Should().BeFalse();
            user!.Locked.Should().BeFalse();
            user!.Email.Should().Be(command.Email);
            user!.UnsuccessfulLoginAttempts.Should().Be(0);
            user!.PasswordHash.Should().NotBe(command.Password);
        });
    }

    [Theory, CustomizedAutoData]
    public async Task Registers_User_With_Same_Email_Once(UserRegisterCommand command)
    {
        // Act
        HttpResponseMessage response1 = await _fixture.Module.Client.PostAsJsonAsync("auth/register", command);
        HttpResponseMessage response2 = await _fixture.Module.Client.PostAsJsonAsync("auth/register", command); 
        
        // Assert
        response1.IsSuccessStatusCode.Should().BeTrue();
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        
        response2.IsSuccessStatusCode.Should().BeTrue();
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            int usersCount = await context.Users.CountAsync(u => u.Email == command.Email);
            usersCount.Should().Be(1);
        });        
    }
    
    [Theory, CustomizedAutoData]
    public async Task Returns_400_When_Request_Missing_Email(UserRegisterCommand command)
    {
        // Arrange
        command.Email = null;
        
        // Act
        HttpResponseMessage response = await _fixture.Module.Client.PostAsJsonAsync("auth/register", command);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            int usersCount = await context.Users.CountAsync(u => u.Email == command.Email);
            usersCount.Should().Be(0);
        });        
    }
    
    [Theory, CustomizedAutoData]
    public async Task Returns_400_When_Request_Missing_Password(UserRegisterCommand command)
    {
        // Arrange
        command.Password = null;
        
        // Act
        HttpResponseMessage response = await _fixture.Module.Client.PostAsJsonAsync("auth/register", command);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            int usersCount = await context.Users.CountAsync(u => u.Email == command.Email);
            usersCount.Should().Be(0);
        });        
    }

    [Theory, CustomizedAutoData]
    public async Task Logs_In(UserRegisterCommand register)
    {
        // Arrange
        await _fixture.Module.Client.PostAsJsonAsync("auth/register", register);
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            user!.EmailConfirmed = true;

            context.Update(user);
            await context.SaveChangesAsync();
        });

        LoginCommand login = new()
        {
            Email    = register.Email,
            Password = register.Password
        };

        // Act
        HttpResponseMessage response = await _fixture.Module.Client.PostAsJsonAsync("auth/login", login);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string cookieValue = response.GetCookie(".AspNetCore.Cookies", "/");
        cookieValue.Should().NotBeNullOrWhiteSpace();
    }

    [Theory, CustomizedAutoData]
    public async Task Raises_Unsuccessful_Login_Attempts_Count(UserRegisterCommand register)
    {
        // Arrange
        await _fixture.Module.Client.PostAsJsonAsync("auth/register", register);
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            user!.EmailConfirmed = true;

            context.Update(user);
            await context.SaveChangesAsync();
        });

        LoginCommand login = new()
        {
            Email    = register.Email,
            Password = "incorrect-password"
        }; 
        
        // Act
        await _fixture.Module.Client.PostAsJsonAsync("auth/login", login);
        
        // Assert
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            user!.UnsuccessfulLoginAttempts.Should().Be(1);
            user!.Locked.Should().BeFalse();
        });
    }
    
    [Theory, CustomizedAutoData]
    public async Task Locks_User_After_Three_Unsuccessful_Logins(UserRegisterCommand register)
    {
        // Arrange
        await _fixture.Module.Client.PostAsJsonAsync("auth/register", register);
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            user!.EmailConfirmed = true;

            context.Update(user);
            await context.SaveChangesAsync();
        });

        LoginCommand login = new()
        {
            Email    = register.Email,
            Password = "incorrect-password"
        }; 
        
        // Act
        await _fixture.Module.Client.PostAsJsonAsync("auth/login", login);
        await _fixture.Module.Client.PostAsJsonAsync("auth/login", login);
        await _fixture.Module.Client.PostAsJsonAsync("auth/login", login);
        
        // Assert
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            user!.UnsuccessfulLoginAttempts.Should().Be(3);
            user!.Locked.Should().BeTrue();
        });
    }
    
    [Theory, CustomizedAutoData]
    public async Task Logs_Out(UserRegisterCommand register)
    {
        // Arrange
        await _fixture.Module.Client.PostAsJsonAsync("auth/register", register);
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            user!.EmailConfirmed = true;

            context.Update(user);
            await context.SaveChangesAsync();
        });
        
        await _fixture.Module.Client.PostAsJsonAsync
        (
            "auth/login", 
            new LoginCommand 
            { 
                Email    = register.Email, 
                Password = register.Password 
            }
        );

        // Act
        HttpResponseMessage response = await _fixture.Module.Client.PostAsync("auth/logout", null);
            
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response
            .Headers
            .GetValues("Set-Cookie")
            .Any(c => c.Contains(".AspNetCore.Cookies"))
            .Should()
            .BeTrue();
        
        response
            .GetCookie(".AspNetCore.Cookies", "/")
            .Should()
            .BeNullOrWhiteSpace();
    }
}