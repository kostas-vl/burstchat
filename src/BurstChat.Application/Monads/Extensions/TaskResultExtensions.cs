using System;
using System.Threading.Tasks;

namespace BurstChat.Application.Monads;

public static class TaskResultExtensions
{
    public static async Task<Result<V>> AndAsync<T, V>(this Task<Result<T>> source, Result<V> target)
    {
        var res = await source;
        return res.And(target);
    }

    public static async Task<Result<V>> AndAsync<T, V>(this Task<Result<T>> source, Task<Result<V>> target)
    {
        var res = await source;
        return await res.AndAsync<V>(target);
    }

    public static async Task<Result<V>> AndAsync<T, V>(this Task<Result<T>> source, Func<T, Result<V>> callback)
    {
        var res = await source;
        return res.And<V>(callback);
    }

    public static async Task<Result<V>> AndAsync<T, V>(this Task<Result<T>> source, Func<T, Task<Result<V>>> callback)
    {
        var res = await source;
        return await res.AndAsync<V>(callback);
    }

    public static async Task<Result<T>> OrAsync<T>(this Task<Result<T>> source, Result<T> target)
    {
        var res = await source;
        return res.Or(target);
    }

    public static async Task<Result<T>> OrAsync<T>(this Task<Result<T>> source, Task<Result<T>> target)
    {
        var res = await source;
        return await res.OrAsync(target);
    }

    public static async Task<Result<T>> OrAsync<T>(this Task<Result<T>> source, Func<Result<T>> callback)
    {
        var res = await source;
        return res.Or(callback);
    }

    public static async Task<Result<T>> OrAsync<T>(this Task<Result<T>> source, Func<Task<Result<T>>> callback)
    {
        var res = await source;
        return await res.OrAsync(callback);
    }

    public static async Task<Result<V>> MapAsync<T, V>(this Task<Result<T>> source, Func<T, V> callback)
    {
        var res = await source;
        return res.Map(callback);
    }

    public static async Task<Result<V>> MapAsync<T, V>(this Task<Result<T>> source, Func<T, Task<V>> callback)
    {
        var res = await source;
        return await res.MapAsync(callback);
    }

    public static async Task<Result<T>> InspectAsync<T>(this Task<Result<T>> source, Func<T, Task> callback)
    {
        var res = await source;
        return await res.InspectAsync(callback);
    }

    public static async Task<Result<T>> InspectErr<T>(this Task<Result<T>> source, Func<MonadException, Task> callback)
    {
        var res = await source;
        return await res.InspectErrAsync(callback);
    }

    public static async Task<T> ExpectAsync<T>(this Task<Result<T>> source, string message)
    {
        var res = await source;
        return res.Expect(message);
    }

    public static async Task<T> UnwrapAsync<T>(this Task<Result<T>> source)
    {
        var res = await source;
        return res.Unwrap();
    }

    public static async Task<T> UnwrapOrAsync<T>(this Task<Result<T>> source, T alternativeValue)
    {
        var res = await source;
        return res.UnwrapOr(alternativeValue);
    }

    public static async Task<T> UnwrapOrAsync<T>(this Task<Result<T>> source, Func<T> callback)
    {
        var res = await source;
        return res.UnwrapOr(callback);
    }
}
