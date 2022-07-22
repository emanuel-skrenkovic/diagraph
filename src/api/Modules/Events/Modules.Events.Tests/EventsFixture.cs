using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Integrations.Google.Fit;
using Diagraph.Infrastructure.Notifications;
using Diagraph.Infrastructure.Tests;
using Diagraph.Infrastructure.Time.Extensions;
using Diagraph.Modules.Events.Database;
using Google.Apis.Fitness.v1.Data;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using DataPoint = Diagraph.Infrastructure.Integrations.Google.Fit.Contracts.DataPoint;

namespace Diagraph.Modules.Events.Tests;

public class EventsFixture : DatabaseModuleFixture<EventsDbContext>, IAsyncLifetime
{
    public static readonly Guid RegisteredUserId = new("f0e7608f-e19a-450d-af66-ab9466f0a7fe");
    
    public EventStoreModuleFixture EventStoreFixture { get; }

    public EventsFixture() : base("events", ConfigureServices) 
        => EventStoreFixture = new("events"); 

    private static void ConfigureServices(IServiceCollection services)
    {
        Mock<IUserContext> userContextMock = new(); 
        userContextMock.SetupGet(c => c.UserId).Returns(RegisteredUserId);
        services.AddScoped(_ => userContextMock.Object);
        
        Mock<INotificationScheduler> notificationSchedulerMock = new();
        notificationSchedulerMock
            .Setup(s => s.ScheduleAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);
         
        services.AddScoped(_ => notificationSchedulerMock.Object);

        Mock<IGoogleFit> googleFitMock = new();
        googleFitMock
            .Setup(f => f.GetActivitiesAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync
            (
                new List<DataPoint>
                {
                    new DataPoint
                    {
                        StartTimeNanos = DateTime.UtcNow.ToUnixTimeNanoseconds().ToString(),
                        EndTimeNanos   = DateTime.UtcNow.AddHours(2).ToUnixTimeNanoseconds().ToString(), 
                        Value = new List<Value>
                        {
                            new Value
                            {
                                IntVal = 7
                            }
                        }
                    },
                    new DataPoint
                    {
                        StartTimeNanos = DateTime.UtcNow.ToUnixTimeNanoseconds().ToString(),
                        EndTimeNanos   = DateTime.UtcNow.AddHours(1).ToUnixTimeNanoseconds().ToString(),
                        Value = new List<Value>
                        {
                            new Value
                            {
                                IntVal = 1
                            }
                        }
                    }
                }
            );
        services.AddScoped(_ => googleFitMock.Object);
    }
    
    public new Task InitializeAsync() => Task.WhenAll
    (
        base.InitializeAsync(),
        EventStoreFixture.InitializeAsync()
    );

    public new Task DisposeAsync() => Task.WhenAll
    (
        base.DisposeAsync(),
        EventStoreFixture.DisposeAsync()
    );
}