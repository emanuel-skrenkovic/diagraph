using Diagraph.Infrastructure.Auth;

namespace Diagraph.Modules.Identity.Api;

public class UserContext : IUserContext
{
    public Guid UserId { get; internal set; }
}