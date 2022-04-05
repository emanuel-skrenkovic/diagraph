namespace Diagraph.Api;

public class UserContext : IUserContext
{
    public Guid UserId { get; internal set; }
}