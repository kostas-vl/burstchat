using System;
using System.Threading.Tasks;

namespace BurstChat.Application.Monads;

/// <summary>
/// This class represent a monda with 2 possible values of Ok or Err.
/// </summary>
public abstract record Result<T>
{
    public abstract bool IsOk { get; }

    public abstract bool IsErr { get; }

    public abstract Result<V> And<V>(Result<V> res);

    public abstract Result<V> And<V>(Func<T, Result<V>> callback);

    public abstract Task<Result<V>> AndAsync<V>(Task<Result<V>> res);

    public abstract Task<Result<V>> AndAsync<V>(Func<T, Task<Result<V>>> callback);

    public abstract Result<T> Or(Result<T> res);

    public abstract Result<T> Or(Func<Result<T>> callback);

    public abstract Task<Result<T>> OrAsync(Task<Result<T>> res);

    public abstract Task<Result<T>> OrAsync(Func<Task<Result<T>>> callback);

    public abstract Result<V> Map<V>(Func<T, V> callback);

    public abstract Task<Result<V>> MapAsync<V>(Func<T, Task<V>> callback);

    public abstract Result<T> MapErr(Func<MonadException, MonadException> callback);

    public abstract Task<Result<T>> MapErrAsync(
        Func<MonadException, Task<MonadException>> callback
    );

    public abstract Result<T> Inspect(Action<T> callback);

    public abstract Task<Result<T>> InspectAsync(Func<T, Task> callback);

    public abstract Result<T> InspectErr(Action<MonadException> callback);

    public abstract Task<Result<T>> InspectErrAsync(Func<MonadException, Task> callback);

    public abstract T Expect(string message);

    public abstract T Unwrap();

    public abstract T UnwrapOr(T alternativeValue);

    public abstract T UnwrapOr(Func<T> callback);

    public static implicit operator Result<T>(MonadException instance) => instance.Err<T>();
}
