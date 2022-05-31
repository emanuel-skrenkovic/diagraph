namespace Diagraph.Infrastructure.Notifications;

public class Notification
{
    public DateTime NotifyAtUtc { get; set; }
    
    public string Text { get; set; }
    
    public string Parent { get; set; }
}