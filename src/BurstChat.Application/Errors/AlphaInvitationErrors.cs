using BurstChat.Application.Monads;
using Level = BurstChat.Application.Monads.ErrorLevel;
using Type = BurstChat.Application.Monads.ErrorType;

namespace BurstChat.Application.Errors;

public static class AlphaInvitationErrors
{
    public static MonadException AlphaInvitationCodeIsNotValid =>
        new(Level.Critical, Type.Validation, "The alpha invitation code is not valid");
}
