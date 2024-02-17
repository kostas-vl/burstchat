using System;
using System.Threading.Tasks;

namespace BurstChat.Application.Monads;

public record Err<T>(MonadException Value) : Result<T>
{
    public override Result<V> And<V>(Result<V> res) =>
        new Err<V>(Value);

    public override Task<Result<V>> AndAsync<V>(Task<Result<V>> res) =>
        Task.FromResult((Result<V>)new Err<V>(Value));

    public override Result<V> And<V>(Func<T, Result<V>> callback) =>
        new Err<V>(Value);

    public override Task<Result<V>> AndAsync<V>(Func<T, Task<Result<V>>> callback) =>
        Task.FromResult((Result<V>)new Err<V>(Value));

    public override Result<T> Or(Result<T> res) => res;

    public override Task<Result<T>> OrAsync(Task<Result<T>> res) => res;

    public override Result<T> Or(Func<Result<T>> callback)
    {
        try
        {
            return callback();
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public override async Task<Result<T>> OrAsync(Func<Task<Result<T>>> callback)
    {
        try
        {
            return await callback();
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public override Result<V> Map<V>(Func<T, V> callback) => new Err<V>(Value);

    public override Task<Result<V>> MapAsync<V>(Func<T, Task<V>> callback) =>
        Task.FromResult((Result<V>)new Err<V>(Value));

    public override Result<T> MapErr(Func<MonadException, MonadException> callback)
    {
        try
        {
            return callback(Value);
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public override async Task<Result<T>> MapErrAsync(Func<MonadException, Task<MonadException>> callback)
    {
        try
        {
            return await callback(Value);
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public override Result<T> Inspect(Action<T> callback) => this;

    public override Task<Result<T>> InspectAsync(Func<T, Task> callback) =>
        Task.FromResult((Result<T>)this);

    public override Result<T> InspectErr(Action<MonadException> callback)
    {
        try
        {
            callback(Value);
            return this;
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public override async Task<Result<T>> InspectErrAsync(Func<MonadException, Task> callback)
    {
        try
        {
            await callback(Value);
            return this;
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public override T Expect(string message) => throw new MonadException(ErrorLevel.Critical, ErrorType.DataProcess, message);

    public override T Unwrap() => throw Value;

    public override T UnwrapOr(T alternativeValue) => alternativeValue;

    public override T UnwrapOr(Func<T> callback) => callback();
}
