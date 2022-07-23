using MailKit.Net.Smtp;
using MimeKit;

namespace Diagraph.Infrastructure.Emails;

public class EmailClient : IEmailClient
{ 
    private readonly EmailServerConfiguration _configuration;

    public EmailClient(EmailServerConfiguration configuration)
        => _configuration = configuration;
    
    public async Task SendAsync(Email email)
    {
        using SmtpClient smtp = new();
        
        await smtp.ConnectAsync
        (
            _configuration.Host, 
            _configuration.Port, 
            false
        );

        await smtp.SendAsync(CreateMessage(email));
        await smtp.DisconnectAsync(true);
    }

    private MimeMessage CreateMessage(Email email)
    {
        MimeMessage message = new()
        {
            Subject = email.Subject,
            Body    = new TextPart(email.IsHtml ? "html" : "plain")
            {
                Text = email.Body
            }
        };

        foreach (EmailAddress emailAddress in email.From)
        {
            message.From.Add(new MailboxAddress("", emailAddress.Address));
        }

        foreach (EmailAddress emailAddress in email.To)
        {
            message.To.Add(new MailboxAddress("", emailAddress.Address));
        }

        return message;
    }
}