using BurstChat.Application.Monads;
using Level = BurstChat.Application.Monads.ErrorLevel;
using Type = BurstChat.Application.Monads.ErrorType;

namespace BurstChat.Application.Errors;

public static class ModelErrors
{
    public static MonadException NameInvalid =>
        new(Level.Critical, Type.Validation, "The user name is not valid");

    public static MonadException EmailInvalid =>
        new(Level.Critical, Type.Validation, "The email is not a valid format");

    public static MonadException PasswordInvalid =>
        new(
            Level.Critical,
            Type.Validation,
            "The password needs to be atleast 12 characters long and contain one character, number and symbol"
        );

    public static MonadException ConfirmPasswordInvalid =>
        new(
            Level.Critical,
            Type.Validation,
            "Both password and confirm password need to be the same"
        );

    public static MonadException OneTimePasswordNotProvided =>
        new(Level.Critical, Type.Validation, "One time password was not provided");

    public static MonadException CredentialsNotProvided =>
        new(Level.Critical, Type.Validation, "Credentials were not provided");

    public static MonadException RegistrationNotProvided =>
        new(Level.Critical, Type.Validation, "Registration information was not provided");

    public static MonadException ChangePasswordNotProvided =>
        new(Level.Critical, Type.Validation, "Password change information was not provided");
}
