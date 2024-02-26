using BurstChat.Application.Monads;

namespace BurstChat.Application.Errors;

public static class ModelErrors
{
    public static MonadException NameInvalid =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The user name is not valid");

    public static MonadException EmailInvalid =>
        new(ErrorLevel.Critical, ErrorType.Validation, "The email is not a valid format");

    public static MonadException PasswordInvalid =>
        new(
            ErrorLevel.Critical,
            ErrorType.Validation,
            "The password needs to be atleast 12 characters long and contain one character, number and symbol"
        );

    public static MonadException ConfirmPasswordInvalid =>
        new(
            ErrorLevel.Critical,
            ErrorType.Validation,
            "Both password and confirm password need to be the same"
        );

    public static MonadException OneTimePasswordNotProvided =>
        new(ErrorLevel.Critical, ErrorType.Validation, "One time password was not provided");

    public static MonadException CredentialsNotProvided =>
        new(ErrorLevel.Critical, ErrorType.Validation, "Credentials were not provided");

    public static MonadException RegistrationNotProvided =>
        new(ErrorLevel.Critical, ErrorType.Validation, "Registration information was not provided");

    public static MonadException ChangePasswordNotProvided =>
        new(ErrorLevel.Critical, ErrorType.Validation, "Password change information was not provided");
}
