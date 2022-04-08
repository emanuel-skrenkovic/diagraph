using MailKit.Net.Smtp;
using MimeKit;

namespace Diagraph.Infrastructure.Emails;

public class EmailClient
{
    private readonly EmailServerConfiguration _configuration;

    public EmailClient(EmailServerConfiguration configuration)
        => _configuration = Ensure.NotNull(configuration);
    
    public async Task SendAsync(Emails.Email email)
    {
        using SmtpClient smtp = new();
        
        await smtp.ConnectAsync(_configuration.Host, _configuration.Port, false); // TODO: configuration
        // await smtp.AuthenticateAsync() // TODO

        await smtp.SendAsync(CreateMessage(email));
        await smtp.DisconnectAsync(true);
    }

    private MimeMessage CreateMessage(Emails.Email email)
    {
        MimeMessage message = new MimeMessage();

        foreach (EmailAddress emailAddress in email.From)
        {
            message.From.Add(new MailboxAddress("", emailAddress.Address));
        }

        foreach (EmailAddress emailAddress in email.To)
        {
            message.To.Add(new MailboxAddress("", emailAddress.Address));
        }

        message.Subject = email.Subject;
        message.Body    = new TextPart(email.IsHtml ? "html" : "plain")
        {
            Text = email.Body
        };
        
        return message;
    }
}