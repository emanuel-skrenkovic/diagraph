using System.Collections.Specialized;
using System.Web;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.ErrorHandling;
using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Auth;

public class UserConfirmationService
{
    private readonly EmailClient _emailClient;
    private readonly EmailServerConfiguration _configuration;

    private readonly DiagraphDbContext _context;

    public UserConfirmationService
    (
        EmailClient emailClient,
        EmailServerConfiguration configuration,
        DiagraphDbContext context
    )
    {
        _emailClient   = emailClient;
        _configuration = configuration;
        _context       = context;
    }
        
    public async Task SendConfirmationEmailAsync(User user, Uri hostUrl)
    {
        Ensure.NotNull(user);
        Ensure.NotNull(hostUrl);
        
        EmailAddress validEmailAddress = EmailAddress.Create(user.Email);

        string token = new AccountConfirmationToken
        {
            UserId        = user.Id,
            SecurityStamp = user.SecurityStamp,
            ExpirationUtc = DateTime.UtcNow.AddDays(1) // TODO: from configuration
        }.ToString();

        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
        query.Add("token", token);
        
        UriBuilder accountConfirmationUrl = new(hostUrl)
        {
            Path  = "/auth/register/confirm", // TODO: configuration
            Query = query.ToString()!
        };

        await _emailClient.SendAsync
        (
            new Email
            (
                from:    new[] { EmailAddress.Create(_configuration.From) },
                to:      new[] { validEmailAddress },
                subject: "Diagraph account",
                body:    $"Please click the link below to activate your account:<br/>"
                         + $"<a href={accountConfirmationUrl}>Confirm</a><br/>"
                         + $"Link:<br/>{accountConfirmationUrl}",
                isHtml:  true
            )
        ); 
    }

    public async Task<Result> ValidateUserConfirmationAsync(string token)
    {
        Ensure.NotNullOrEmpty(token);

        AccountConfirmationToken tokenValues = AccountConfirmationToken.FromString(token);
        User user = await _context.FindAsync<User>(tokenValues.UserId);
        
        if (user == null)                                    return new Error("User was not found");
        if (user.SecurityStamp != tokenValues.SecurityStamp) return new Error("Token security stamp does not match user.");
        if (DateTime.UtcNow > tokenValues.ExpirationUtc)     return new Error("Token has expired");

        user.EmailConfirmed = true;
        user.SecurityStamp  = Guid.NewGuid();

        _context.Update(user);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }
}