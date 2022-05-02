namespace Diagraph.Infrastructure.Emails;

public interface IEmailClient
{
    Task SendAsync(Email email);
}