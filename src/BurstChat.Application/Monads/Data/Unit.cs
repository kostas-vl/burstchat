namespace BurstChat.Application.Monads;

public record struct Unit
{
    public static Unit Instance = new Unit();

    public static Result<Unit> Ok => new Unit().Ok();
}
