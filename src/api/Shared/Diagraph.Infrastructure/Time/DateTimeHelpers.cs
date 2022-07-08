namespace Diagraph.Infrastructure.Time;

public class DateTimeHelpers
{
    public static DateTime FromUnixTimeNanoseconds(long nanoseconds) 
        => DateTimeOffset
            .FromUnixTimeMilliseconds(nanoseconds / 1_000_000)
            .UtcDateTime;
}