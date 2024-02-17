namespace BurstChat.Application.Monads;

public static class ErrExtensions
{
    public static Result<T> Err<T>(this MonadException instance) =>
        new Err<T>(instance);

    public static Result<Unit> Err(this MonadException instance) =>
        new Err<Unit>(instance);
}
