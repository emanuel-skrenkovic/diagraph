using System.Threading.Tasks;
using Diagraph.Infrastructure.Notifications;
using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.Events.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Diagraph.Modules.Events.Tests;

public class EventsFixture : DatabaseModuleFixture<EventsDbContext>
{
    public EventsFixture() : base("events", ConfigureServices) { }

    private static void ConfigureServices(IServiceCollection services)
    {
         Mock<INotificationScheduler> notificationSchedulerMock = new();
         notificationSchedulerMock.Setup
         (
             s => s.ScheduleAsync(It.IsAny<Notification>())
         ).Returns(Task.CompletedTask);
         
         services.AddScoped(_ => notificationSchedulerMock.Object);
    }
}