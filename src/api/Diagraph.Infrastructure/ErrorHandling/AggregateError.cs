namespace Diagraph.Infrastructure.ErrorHandling;

public class AggregateError : Error
{
    public IEnumerable<Error> Errors { get; }

    public AggregateError(string message, IEnumerable<Error> errors) : base(message)
        => Errors = errors;

    public static AggregateError FromException(Exception ex)
    {
        List<Error> errors = new();
        Exception current = ex;

        while (current != null)
        {
            errors.Add
            (
                new(current.Message)
            );
            
            current = current.InnerException;
        }

        return new AggregateError("Aggregate error occurred.", errors);
    }
}