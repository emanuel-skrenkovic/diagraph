namespace Diagraph.Infrastructure.ErrorHandling;

public class Error
{
    public Error(string message) => Message = Ensure.NotNullOrEmpty(message);
    
    public string Message { get; }
    
    public Exception ToException()
    {
        if (this is not AggregateError aggregateError) return new Exception(Message);
        
        return new AggregateException
        (
            aggregateError.Errors.Select(e => new Exception(e.Message))
        );
    }
}