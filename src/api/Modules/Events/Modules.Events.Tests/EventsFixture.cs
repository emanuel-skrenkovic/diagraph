using System;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Notifications;
using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.Events.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Diagraph.Modules.Events.Tests;

public class EventsFixture : DatabaseModuleFixture<EventsDbContext>
{
    public static readonly Guid RegisteredUserId = new("f0e7608f-e19a-450d-af66-ab9466f0a7fe");
    
    public EventsFixture() : base("events", ConfigureServices) { }

    private static void ConfigureServices(IServiceCollection services)
    {
        Mock<IUserContext> userContextMock = new(); 
        userContextMock.SetupGet(c => c.UserId).Returns(RegisteredUserId);
        services.AddScoped(_ => userContextMock.Object);
        
        Mock<INotificationScheduler> notificationSchedulerMock = new();
        notificationSchedulerMock.Setup
        (
            s => s.ScheduleAsync(It.IsAny<Notification>())
        ).Returns(Task.CompletedTask);
         
        services.AddScoped(_ => notificationSchedulerMock.Object);
    }
}