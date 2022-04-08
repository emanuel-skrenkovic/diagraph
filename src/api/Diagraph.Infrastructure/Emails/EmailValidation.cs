using System.Net.Mail;
using Diagraph.Infrastructure.ErrorHandling;

namespace Diagraph.Infrastructure.Emails;

public static class EmailValidation
{
    public static Result Run(string email)
    {
        Ensure.NotNullOrEmpty(email);

        try
        {
            MailAddress mailAddress = new MailAddress(email);

            if (email != mailAddress.Address)
            {
                return new Error(
                    $"Provided email address ('{email}') does not match the parsed address ('{mailAddress.Address}').");
            }

            return Result.Ok();
        }
        catch (FormatException formatException)
        {
            return formatException.InnerException is not null 
                ? AggregateError.FromException(formatException) 
                : new Error(formatException.Message);
        }
    }
}