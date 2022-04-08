using MailKit.Net.Smtp;
using MimeKit;

namespace Diagraph.Infrastructure.Email;

public class EmailClient
{
    public async Task SendAsync(Email email)
    {
        using SmtpClient smtp = new SmtpClient();
        
        await smtp.ConnectAsync("", 587, false); // TODO: configuration
        // await smtp.AuthenticateAsync() // TODO

        await smtp.SendAsync(FromEmail(email));
        await smtp.DisconnectAsync(true);
    }

    // TODO: this is business logic related.
    private MimeMessage FromEmail(Email email)
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