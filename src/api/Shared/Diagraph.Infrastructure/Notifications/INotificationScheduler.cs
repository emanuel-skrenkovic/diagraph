namespace Diagraph.Infrastructure.Notifications;

public interface INotificationScheduler
{
    Task ScheduleAsync(Notification notification);
}