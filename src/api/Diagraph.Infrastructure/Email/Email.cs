using System.Collections.Immutable;

namespace Diagraph.Infrastructure.Email;

public class Email
{
    public IReadOnlyCollection<EmailAddress> From { get; }
    
    public IReadOnlyCollection<EmailAddress> To { get; }
    
    public string Subject { get; }
    
    public string Body { get; }
    
    public bool IsHtml { get; }

    public Email
    (
        IEnumerable<EmailAddress> from,
        IEnumerable<EmailAddress> to,
        string subject = null,
        string body = null,
        bool isHtml = false
    )
    {
        Ensure.NotNullOrEmpty(from);
        Ensure.NotNullOrEmpty(to);
        
        From = from.ToImmutableArray();
        To = to.ToImmutableArray();
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
    }
}