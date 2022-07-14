namespace Diagraph.Infrastructure.Retries;

public class RetryConfiguration
{
    public int MaxRetryCount { get; set; } = 5;

    public int RetryDelayMilliseconds { get; set; } = 500;
}