using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Diagraph.Infrastructure.Auth;

public record AccountConfirmationToken
{
    public Guid UserId { get; set; }
    public Guid SecurityStamp { get; set; }
    public DateTime ExpirationUtc { get; set; }

    public override string ToString() =>
        Convert.ToBase64String
        (
            Encoding.UTF8.GetBytes($"{UserId}:{SecurityStamp}:{ExpirationUtc.Ticks}")
        );

    public static AccountConfirmationToken FromString(string token)
    {
        Ensure.NotNullOrEmpty(token);
        string[] tokenParts = Encoding.UTF8.GetString
        (
            Convert.FromBase64String(token)
        ).Split(':');

        if (tokenParts.Length != 3) throw new ValidationException("Confirmation Token: incorrect format.");
            
        if (!Guid.TryParse(tokenParts[0], out Guid userId))
        {
            throw new ValidationException("Confirmation Token: user id in incorrect format.");
        }

        if (!Guid.TryParse(tokenParts[1], out Guid securityStamp))
        {
            throw new ValidationException("Confirmation Token: security stamp in incorrect format."); 
        }

        if (!long.TryParse(tokenParts[2], out long expirationUtc))
        {
            throw new ValidationException("Confirmation Token: expiration in incorrect format.");
        }

        return new AccountConfirmationToken
        {
            UserId = userId,
            SecurityStamp = securityStamp,
            ExpirationUtc = new DateTime(expirationUtc)
        };
    }
}