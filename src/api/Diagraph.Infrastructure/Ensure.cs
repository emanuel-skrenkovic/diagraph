using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Diagraph.Infrastructure;

public static class Ensure
{
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(T arg, [CallerArgumentExpression("arg")] string argName = null) 
        => arg ?? throw new ArgumentNullException(argName?.TrimStart('@'));
        
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NotNullOrEmpty(string arg, string argName)
    {
        if (string.IsNullOrWhiteSpace(arg))
        {
            throw new ArgumentException(CommonMessages.NullOrEmpty(argName));
        }
    }
    
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNullOrEmpty<T>(T arg, [CallerArgumentExpression("arg")] string argName = null) where T : IEnumerable
    {
        if (arg?.GetEnumerator().MoveNext() != true)
        {
            throw new ArgumentException(CommonMessages.NullOrEmpty(argName?.TrimStart('@')));
        }

        return arg;
    }

    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal NonNegative(decimal number, [CallerArgumentExpression("number")] string argName = null)
    {
        if (number < 0)
        {
            throw new ArgumentException($"{argName} is negative.");
        }

        return number;
    }
    
    private static class CommonMessages
    {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NullOrEmpty(string argName) => $"{argName} is null or empty.";
    }
}