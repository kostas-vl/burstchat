using BurstChat.Application.Monads;
using Level = BurstChat.Application.Monads.ErrorLevel;
using Type = BurstChat.Application.Monads.ErrorType;

namespace BurstChat.Application.Errors;

public static class UserErrors
{
    public static MonadException UserNotFound =>
        new(Level.Critical, Type.Validation, "The user was not found");

    public static MonadException UserAlreadyExists =>
        new(Level.Critical, Type.Validation, "The email is already registered");

    public static MonadException UserPasswordDidNotMatch =>
        new(Level.Critical, Type.Validation, "The email or password is incorrect");

    public static MonadException UserOneTimePasswordInvalid =>
        new(Level.Critical, Type.Validation, "The one time password is invalid");

    public static Error UserOneTimePasswordExpired() => new(ErrorLevel.Critical,
                                                            ErrorType.Validation,
                                                            "The one time password has expired");

    public static MonadException CouldNotUpdateInvitation =>
        new(Level.Critical, Type.DataProcess, "Could not update invitation");
}
