using System;
using System.Threading.Tasks;

namespace BurstChat.Application.Monads;

public static class ResultExtensions
{
    public static Result<V> And<T, V>(this T instance, Result<V> res) => res;

    public static Result<V> And<T, V>(this Exception instance, Result<V> res) =>
        new ResultCallbackException(instance);

    public static Result<V> And<T, V>(this T instance, Func<T, Result<V>> callback)
    {
        try
        {
            return callback(instance);
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public static Result<V> And<T, V>(this Exception instance, Func<T, Result<V>> callback) =>
        new ResultCallbackException(instance);

    public static Task<Result<V>> AndAsync<T, V>(this T instance, Task<Result<V>> res) => res;

    public static Task<Result<V>> AndAsync<T, V>(this Exception instance, Task<Result<V>> res) =>
        Task.FromResult(new ResultCallbackException(instance).Err<V>());

    public static async Task<Result<V>> AndAsync<T, V>(this T instance, Func<T, Task<Result<V>>> callback)
    {
        try
        {
            return await callback(instance);
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public static Task<Result<V>> AndAsync<T, V>(this Exception instance, Func<T, Task<Result<V>>> callback) =>
        Task.FromResult(new ResultCallbackException(instance).Err<V>());

    public static Result<V> Map<T, V>(this T instance, Func<T, V> callback)
    {
        try
        {
            return callback(instance).Ok();
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public static Result<V> Map<T, V>(this Exception instance, Func<T, V> callback) =>
        new ResultCallbackException(instance).Err<V>();

    public static async Task<Result<V>> MapAsync<T, V>(this T instance, Func<T, Task<V>> callback)
    {
        try
        {
            return (await callback(instance)).Ok();
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public static Task<Result<V>> MapAsync<T, V>(this Exception instance, Func<T, Task<V>> callback) =>
        Task.FromResult(new ResultCallbackException(instance).Err<V>());

    public static Result<T> Inspect<T>(this T instance, Action<T> callback)
    {
        try
        {
            callback(instance);
            return instance.Ok();
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public static async Task<Result<T>> InspectAsync<T>(this T instance, Func<T, Task> callback)
    {
        try
        {
            await callback(instance);
            return instance.Ok();
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public static Result<T> InspectErr<T>(this Exception instance, Action<Exception> callback)
    {
        try
        {
            callback(instance);
            return new MonadException(
                ErrorLevel.Critical, ErrorType.DataProcess, "Wrapper exception see inner exception for more detauls", instance);
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }

    public static async Task<Result<T>> InspectAsync<T>(this Exception instance, Func<Exception, Task> callback)
    {
        try
        {
            await callback(instance);
            return new MonadException(
                ErrorLevel.Critical, ErrorType.DataProcess, "Wrapper exception see inner exception for more detauls", instance);
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }
}
