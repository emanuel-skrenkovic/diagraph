using Diagraph.Core.Database;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.Models.ValueObjects;

namespace Diagraph.Infrastructure.Models;

public record AuthResult(bool Authenticated, string Reason = null);

public class User : DbEntity
{
    public Guid Id { get; set; }
    
    public Guid SecurityStamp { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    
    public bool Locked { get; set; }
    
    public int UnsuccessfulLoginAttempts { get; set; }
    
    public bool EmailConfirmed { get; set; }

    public AuthResult Authenticate(string providedPassword, PasswordTool passwordTool)
    {
        if (!passwordTool.Compare(PasswordHash, providedPassword))
        {
            Locked = ++UnsuccessfulLoginAttempts >= 3;

            string reason;
            if (Locked)
            {
                SecurityStamp = Guid.NewGuid();
                reason = "Account has been locked.";
            }
            else
            {
                reason = "Invalid user or password";
            }

            return new(false, reason);
        }

        UnsuccessfulLoginAttempts = 0;

        return new(true);
    }

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