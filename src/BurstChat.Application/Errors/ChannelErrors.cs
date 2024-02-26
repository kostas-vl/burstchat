using BurstChat.Application.Monads;

namespace BurstChat.Application.Errors;

public static class ChannelErrors
{
    public static MonadException ChannelNotFound =>
        new(ErrorLevel.Critical, ErrorType.DataProcess, "The channel was not found");

    public static MonadException ChannelMessageNotFound =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The channel message was not found");
}
