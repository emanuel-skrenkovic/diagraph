using MediatR;

namespace Diagraph.Infrastructure.Integrations.Google.InterModuleIntegration;

public record AccessTokenRefreshedNotification(Guid UserId, string AccessToken) : INotification;