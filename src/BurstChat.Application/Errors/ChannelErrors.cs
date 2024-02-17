using BurstChat.Application.Monads;
using Level = BurstChat.Application.Monads.ErrorLevel;
using Type = BurstChat.Application.Monads.ErrorType;

namespace BurstChat.Application.Errors;

public static class ChannelErrors
{
    public static MonadException ChannelNotFound =>
        new(Level.Critical, Type.DataProcess, "The channel was not found");

    public static MonadException ChannelMessageNotFound =>
        new(Level.Critical, Type.Validation, "The channel message was not found");
}
