namespace Diagraph.Infrastructure.Time.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnixTimeNanoseconds(this DateTime dt)
        => new DateTimeOffset(dt).ToUnixTimeMilliseconds() * 1_000_000;
}