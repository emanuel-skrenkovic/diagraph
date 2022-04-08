using Diagraph.Core.Database;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.Models.ValueObjects;

namespace Diagraph.Infrastructure.Models;

public class User : DbEntity
{
    public Guid Id { get; set; }
    
    public Guid SecurityStamp { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    
    public bool Locked { get; set; }
    
    public int UnsuccessfulLoginAttempts { get; set; }
    
    public bool EmailConfirmed { get; set; }

    public static User Create
    (
        EmailAddress email, Password password, PasswordTool passwordTool
    ) => new()
        {
            Id = Guid.NewGuid(),
            SecurityStamp = Guid.NewGuid(),
            Email = email.Address,
            PasswordHash = passwordTool.Hash(password.Value)
        };
}