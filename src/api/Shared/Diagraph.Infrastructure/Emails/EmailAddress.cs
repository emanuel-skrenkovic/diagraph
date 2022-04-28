namespace Diagraph.Infrastructure.Emails;

public class EmailAddress : ValueObject<EmailAddress>
{
    public string Address { get; }

    private EmailAddress(string emailAddress) => Address = emailAddress;

    public static EmailAddress Create(string emailAddress)
    {
        Ensure.NotNullOrEmpty(emailAddress);
        EmailValidation.Run(emailAddress).UnwrapOrElse(e => throw e.ToException());

        return new EmailAddress(emailAddress);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}