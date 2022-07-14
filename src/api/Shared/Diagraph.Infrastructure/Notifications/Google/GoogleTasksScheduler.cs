using System.Net.Http.Json;
using System.Xml;
using Diagraph.Infrastructure.Integrations.Google;
using Google.Apis.Tasks.v1.Data;
using GoogleTask = Google.Apis.Tasks.v1.Data.Task;
using Task = System.Threading.Tasks.Task;

namespace Diagraph.Infrastructure.Notifications.Google;

// TODO: Cannot find a way to schedule a task at a time,
// the Calendar API does not support tasks, nor reminders.
// This is wholly pointless until something changes.
public class GoogleTasksScheduler : INotificationScheduler
{
    private readonly IHttpClientFactory _clientFactory;

    public GoogleTasksScheduler(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;
    
    public async Task ScheduleAsync(Notification notification)
    {
        HttpClient client = _clientFactory.CreateClient(GoogleIntegrationConsts.TasksClientName);
        
        TaskLists tasksListResponse = await client
            .GetFromJsonAsync<TaskLists>("/tasks/v1/users/@me/lists");

        // TODO: parameter
        string taskList = tasksListResponse
            ?.Items
            .FirstOrDefault(l => l.Title == notification.Parent)
            ?.Id;
        
        if (string.IsNullOrWhiteSpace(taskList))
            throw new InvalidOperationException($"Could not find {notification.Parent} task list.");

        GoogleTask task = new()
        {
            Title = notification.Text,
            Due   = notification.NotifyAtUtc > DateTime.MinValue 
                ? XmlConvert
                    .ToString(notification.NotifyAtUtc.ToLocalTime(), XmlDateTimeSerializationMode.Local)
                : null
        };

        HttpResponseMessage response = await client.PostAsJsonAsync
        (
            $"/tasks/v1/lists/{taskList}/tasks",
            task
        );
        response.EnsureSuccessStatusCode(); // TODO: proper error handling
    }
}