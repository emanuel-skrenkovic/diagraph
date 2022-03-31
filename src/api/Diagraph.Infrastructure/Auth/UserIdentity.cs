using System.Security.Principal;
using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Auth;

public class UserIdentity : IIdentity
{
    public string? AuthenticationType { get; private set; }
    
    public bool IsAuthenticated { get; private set; }
    
    public string? Name { get; private set; }

    private UserIdentity() {}

    public static UserIdentity Create(User user, string authenticationType)
        => new()
        {
            Name = user.Id.ToString(),
            AuthenticationType = authenticationType,
            IsAuthenticated = true
        };
}