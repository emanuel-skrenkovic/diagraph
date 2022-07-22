namespace Diagraph.Infrastructure.Retries;

public class RetryCounter
{
    private readonly RetryConfiguration _configuration;
    
    private int                      _retryCount;
    private readonly List<Exception> _exceptions = new();

    public RetryCounter(RetryConfiguration configuration) => _configuration = configuration;
    
    public async Task RetryAsync(Func<Task> retryAction)
    {
        while (_retryCount < _configuration.MaxRetryCount)
        {
            await Task.Delay(_configuration.RetryDelayMilliseconds);
            
            try
            {
                await retryAction();

                _retryCount = 0;
                _exceptions.Clear();
                return;
            }
            catch (Exception ex)
            {
                _exceptions.Add(ex);
                Interlocked.Increment(ref _retryCount);
            } 
        }

        throw new AggregateException
        (
            $"Failed to connect after {_retryCount} attempts.", 
            _exceptions
        ); 
    }
}