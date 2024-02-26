using BurstChat.Application.Monads;

namespace BurstChat.Application.Errors;

public static class DirectMessagingErrors
{
    public static MonadException DirectMessagingNotFound =>
        new MonadException(ErrorLevel.Critical, ErrorType.DataProcess, "The direct messages dont exist");

    public static MonadException DirectMessagingAlreadyExists =>
        new MonadException(
            ErrorLevel.Critical,
            ErrorType.DataProcess,
            "The direct messages have already been associated"
        );

    public static MonadException DirectMessagesNotFound =>
        new MonadException(
            ErrorLevel.Critical,
            ErrorType.DataProcess,
            "The direct messages were not found"
        );

    public static MonadException DirectMessageNotFound =>
        new MonadException(
            ErrorLevel.Critical,
            ErrorType.DataProcess,
            "The direct message was not found"
        );
}
