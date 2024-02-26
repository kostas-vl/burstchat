using BurstChat.Application.Monads;

namespace BurstChat.Application.Errors;

public static class AlphaInvitationErrors
{
    public static MonadException AlphaInvitationCodeIsNotValid =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The alpha invitation code is not valid");
}
