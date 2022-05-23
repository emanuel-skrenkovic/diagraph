using System;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.Identity.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Diagraph.Modules.Identity.Tests;

public class IdentityFixture : DatabaseModuleFixture<IdentityDbContext>
{
    public static readonly Guid RegisteredUserId = new("f0e7608f-e19a-450d-af66-ab9466f0a7fe");
    
    public IdentityFixture() : base("identity", ConfigureServices) { }

    private static void ConfigureServices(IServiceCollection services)
    {
        Mock<IEmailClient> emailClientMock = new();
        emailClientMock
            .Setup(c => c.SendAsync(It.IsAny<Email>()))
            .Returns(Task.CompletedTask);

        Mock<IUserContext> userContextMock = new();
        userContextMock.SetupGet(c => c.UserId).Returns(RegisteredUserId);

        services.AddScoped(_ => emailClientMock.Object);
        services.AddScoped(_ => userContextMock.Object);
    }
}