namespace Diagraph.Infrastructure.Retries;

public class RetryConfiguration
{
    public int MaxRetryCount { get; init; }

    public int RetryDelayMilliseconds { get; init;  }

    public static RetryConfiguration Default
        => new() { MaxRetryCount = 15, RetryDelayMilliseconds = 500 };
}