using BurstChat.Application.Monads;

namespace BurstChat.Application.Errors;

public static class PrivateGroupErrors
{
    public static MonadException GroupNotFound =>
        new(ErrorLevel.Critical, ErrorType.DataProcess, "The group does not exist");

    public static MonadException GroupMessageNotFound =>
        new(ErrorLevel.Critical, ErrorType.DataProcess, "The group message does not exist");
}
