namespace Diagraph.Infrastructure;

public abstract class ValueObject<T> : IEquatable<T>
{
    protected abstract IEnumerable<object> GetEqualityComponents();
        
    #region GetHashCode
        
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 486187739; // A nice, healthy prime number. :)
                
            foreach (object component in GetEqualityComponents())
            {
                hash = hash * 23 + component?.GetHashCode() ?? 0;
            }
                
            return hash;
        }
    }
        
    #endregion
        
    #region Equality

    public bool Equals(T other)
    {
        return Equals((object)other);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)                     return false;
        if (obj.GetType() != this.GetType()) return false;
        if (ReferenceEquals(this, obj))      return true;

        ValueObject<T> otherValueObject = (ValueObject<T>)obj;

        return GetEqualityComponents()
            .SequenceEqual(otherValueObject.GetEqualityComponents());
    }

    public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
    {
        // Use 'is' to avoid the overloaded equality operator.
        if (left is null && right is null) return true;

        // We already ruled out that both are null,
        // so if only one is, they are not equal.
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
    {
        // Make use of the overloaded equality operator.
        return !(left == right);
    }
        
    #endregion
}