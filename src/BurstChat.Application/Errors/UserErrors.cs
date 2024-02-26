using BurstChat.Application.Monads;

namespace BurstChat.Application.Errors;

public static class UserErrors
{
    public static MonadException UserNotFound =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The user was not found");

    public static MonadException UserAlreadyExists =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The email is already registered");

    public static MonadException UserPasswordDidNotMatch =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The email or password is incorrect");

    public static MonadException UserOneTimePasswordInvalid =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The one time password is invalid");

    public static MonadException UserOneTimePasswordExpired() =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The one time password has expired");

    public static MonadException CouldNotUpdateInvitation =>
        new(ErrorLevel.Critical, ErrorType.DataProcess, "Could not update invitation");
}
