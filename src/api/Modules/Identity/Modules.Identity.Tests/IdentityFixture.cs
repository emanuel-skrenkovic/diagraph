using System.Threading.Tasks;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.Identity.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Diagraph.Modules.Identity.Tests;

public class IdentityFixture : DatabaseModuleFixture<IdentityDbContext>
{
    public IdentityFixture() : base("identity", ConfigureServices) { }

    private static void ConfigureServices(IServiceCollection services)
    {
        Mock<IEmailClient> emailClientMock = new();
        emailClientMock
            .Setup(c => c.SendAsync(It.IsAny<Email>()))
            .Returns(Task.CompletedTask);

        services.AddScoped(_ => emailClientMock.Object);
    }
}