using System;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.GlucoseData.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Diagraph.Modules.GlucoseData.Tests;

public class GlucoseDataFixture : DatabaseModuleFixture<GlucoseDataDbContext>
{
    public static readonly Guid RegisteredUserId = new("f0e7608f-e19a-450d-af66-ab9466f0a7fe"); 
    
    public GlucoseDataFixture() : base("glucose-data", ConfigureServices) { }

    private static void ConfigureServices(IServiceCollection services)
    {
        Mock<IUserContext> userContextMock = new();
        userContextMock.SetupGet(c => c.UserId).Returns(RegisteredUserId);
        services.AddScoped(_ => userContextMock.Object);
    }
}