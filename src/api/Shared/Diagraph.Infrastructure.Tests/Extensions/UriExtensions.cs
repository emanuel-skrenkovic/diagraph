using System;
using System.Linq;

namespace Diagraph.Infrastructure.Tests.Extensions;

public static class UriExtensions
{
    public static T AsId<T>(this Uri location)
    {
        string last = location.ToString().Split('/').Last();

        if (typeof(T) == typeof(int))
        {
            return (T)(object)int.Parse(last); // Beauty
        }

        if (typeof(T) == typeof(Guid))
        {
            return (T)(object)Guid.Parse(last);
        }
        
        throw new ArgumentOutOfRangeException(nameof(T));
    }
}