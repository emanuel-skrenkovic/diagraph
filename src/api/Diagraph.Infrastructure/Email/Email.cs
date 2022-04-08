namespace Diagraph.Infrastructure.Email;

public class Email : ValueObject<Email>
{
    public string Address { get; }

    public static Email Create(string emailAddress)
    {
        Ensure.NotNullOrEmpty(emailAddress);
        EmailValidation.Run(emailAddress).UnwrapOrElse(e => throw e.ToException());

        return new Email(emailAddress);
    }

    private Email(string emailAddress) => Address = emailAddress;
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}