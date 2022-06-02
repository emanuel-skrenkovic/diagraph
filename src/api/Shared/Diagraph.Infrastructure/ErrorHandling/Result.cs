namespace Diagraph.Infrastructure.ErrorHandling;

public class Result
{
    private static readonly Result OkValue = new();

    private protected readonly Error _error;

    public bool IsOk { get; }
    public bool IsError => !IsOk;

    private protected Result() => IsOk = true;

    private protected Result(Error error)
    {
        _error = Ensure.NotNull(error);
        IsOk = false;
    }

    public void Match(Action ok, Action<Error> error)
    {
        if (IsOk)
        {
            ok();
        }
        else
        {
            error(_error);
        }
    }

    public Task Match(Func<Task> ok, Func<Error, Task> error)
        => IsOk ? ok() : error(_error);
    
    public TResult Match<TResult>(Func<TResult> ok, Func<Error, TResult> error)
        => IsOk ? ok() : error(_error);

    public Task<TResult> Match<TResult>(Func<Task<TResult>> ok, Func<Error, Task<TResult>> error)
        => IsOk ? ok() : error(_error);

    public void UnwrapOrElse(Action<Error> @else)
    {
        if (IsOk) return;
        @else(_error);
    } 

    public static Result<T> Ok<T>(T value)
    {
        Ensure.NotNull(value);
        return new(value);
    }

    public static Result Ok() => OkValue;

    public static Result Error(Error error) => new(error);

    public static Result<T> Error<T>(Error error)
    {
        Ensure.NotNull(error);
        return new(error);
    }

    public static implicit operator Result(Error error) => new(error);
}

public sealed class Result<T> : Result
{
    private readonly T _value;

    internal Result(T value)
        => _value = Ensure.NotNull(value);

    internal Result(Error error) : base(error)
    {
    }

    public bool TryGetValue(out T value)
    {
        if (IsError)
        {
            value = default;
            return false;
        }

        value = _value;
        return true;
    }

    public T UnwrapOrDefault() => UnwrapOrDefault(default);

    public T UnwrapOrDefault(T defaultValue) => !TryGetValue(out T value) ? defaultValue : value;
    
    public T UnwrapOrElse(Func<Error, T> @else)
    {
        if (IsOk) return _value;
        return @else(_error);
    } 

    public T Unwrap() => Expect("Cannot unwrap Option<T> without value.");

    public T Expect(string message)
    {
        if (IsError) throw new InvalidOperationException(message);
        return _value;
    }

    public void Match(Action<T> ok, Action<Error> error)
    {
        if (IsOk)
        {
            ok(_value);
        }
        else
        {
            error(_error);
        }
    }

    public Task Match(Func<T, Task<T>> ok, Func<Error, Task> error)
        => IsOk ? ok(_value) : error(_error);

    public TResult Match<TResult>(Func<T, TResult> ok, Func<Error, TResult> error)
        => IsOk ? ok(_value) : error(_error);

    public Task<TResult> Match<TResult>(Func<T, Task<TResult>> ok, Func<Error, Task<TResult>> errors)
        => IsOk ? ok(_value) : errors(_error);

    public static implicit operator Result<T>(Error error) => new(error);
    public static implicit operator Result<T>(T value) => new(value);
}