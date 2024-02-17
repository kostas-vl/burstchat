using BurstChat.Application.Monads;
using Level = BurstChat.Application.Monads.ErrorLevel;
using Type = BurstChat.Application.Monads.ErrorType;

namespace BurstChat.Application.Errors;

public static class PrivateGroupErrors
{
    public static MonadException GroupNotFound =>
        new(Level.Critical, Type.DataProcess, "The group does not exist");

    public static MonadException GroupMessageNotFound =>
        new(Level.Critical, Type.DataProcess, "The group message does not exist");
}
