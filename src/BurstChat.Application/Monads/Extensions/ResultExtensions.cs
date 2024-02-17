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

    public static Result<T> InspectErr<T>(this Exception instance, Action<Exception> callback)
    {
        try
        {
            callback(instance);
            return new MonadException(
                ErrorLevel.Critical,
                ErrorType.DataProcess,
                "Wrapper exception see inner exception for more detauls",
                instance
            );
        }
        catch (Exception ex)
        {
            return new ResultCallbackException(ex);
        }
    }
}
