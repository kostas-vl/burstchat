using System;
using System.Threading.Tasks;

namespace BurstChat.Application.Monads;

public record Ok<T>(T Value) : Result<T>
{
    public override Result<V> And<V>(Result<V> res) => res;

    public override Task<Result<V>> AndAsync<V>(Task<Result<V>> res) => res;

    public override Result<V> And<V>(Func<T, Result<V>> callback)
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

    public override async Task<Result<V>> AndAsync<V>(Func<T, Task<Result<V>>> callback)
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

    public override Result<T> Or(Result<T> res) => this;

    public override Task<Result<T>> OrAsync(Task<Result<T>> res) =>
        Task.FromResult((Result<T>)this);

    public override Result<T> Or(Func<Result<T>> callback) => this;

    public override Task<Result<T>> OrAsync(Func<Task<Result<T>>> callback) =>
        Task.FromResult((Result<T>)this);

    public override Result<V> Map<V>(Func<T, V> callback)
    {
        try
        {
            return new Ok<V>(callback(Value));
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public override async Task<Result<V>> MapAsync<V>(Func<T, Task<V>> callback)
    {
        try
        {
            return new Ok<V>(await callback(Value));
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public override Result<T> MapErr(Func<MonadException, MonadException> callback) => this;

    public override Task<Result<T>> MapErrAsync(Func<MonadException, Task<MonadException>> callback) =>
        Task.FromResult((Result<T>)this);

    public override Result<T> Inspect(Action<T> callback)
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

    public override async Task<Result<T>> InspectAsync(Func<T, Task> callback)
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

    public override Result<T> InspectErr(Action<MonadException> callback) => this;

    public override Task<Result<T>> InspectErrAsync(Func<MonadException, Task> callback) =>
        Task.FromResult((Result<T>)this);

    public override T Expect(string message) => Value;

    public override T Unwrap() => Value;

    public override T UnwrapOr(T alternativeValue) => Value;

    public override T UnwrapOr(Func<T> callback) => Value;
}
