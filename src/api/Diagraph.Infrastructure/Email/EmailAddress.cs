namespace Diagraph.Infrastructure.Email;

public class EmailAddress : ValueObject<EmailAddress>
{
    public string Address { get; }

    public static EmailAddress Create(string emailAddress)
    {
        Ensure.NotNullOrEmpty(emailAddress);
        EmailValidation.Run(emailAddress).UnwrapOrElse(e => throw e.ToException());

        return new EmailAddress(emailAddress);
    }

    private EmailAddress(string emailAddress) => Address = emailAddress;
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}