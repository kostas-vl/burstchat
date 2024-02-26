using BurstChat.Application.Monads;

namespace BurstChat.Application.Errors;

public static class SystemErrors
{
    public static MonadException Exception =>
        new MonadException(
            ErrorLevel.Critical,
            ErrorType.System,
            "One or more actions could not be completed"
        );
}
