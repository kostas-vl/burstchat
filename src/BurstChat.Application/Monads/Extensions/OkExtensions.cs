namespace BurstChat.Application.Monads;

public static class OkExtensions
{
    public static Result<T> Ok<T>(this T instance) => new Ok<T>(instance);
}
