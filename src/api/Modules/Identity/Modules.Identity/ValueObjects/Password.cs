using Diagraph.Infrastructure;

namespace Diagraph.Modules.Identity.ValueObjects;

public class Password : ValueObject<Password>
{
    public string Value { get; }

    private Password(string value) => Value = value;

    public static Password Create(string value)
    {
        Ensure.NotNullOrEmpty(value);
        return new(value);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}