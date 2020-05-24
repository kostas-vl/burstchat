using System;

namespace BurstChat.Application.Errors
{
    /// <summary>
    ///   This class contains static methods that represent errors associated with api model errors when the
    ///   application tries to either fetch or transform data.
    /// </summary>
    public static class ModelErrors
    {
        public static Error NameInvalid() => new Error(ErrorLevel.Critical,
                                                       ErrorType.Validation,
                                                       "The user name is not valid");

        public static Error EmailInvalid() => new Error(ErrorLevel.Critical,
                                                        ErrorType.Validation,
                                                        "The email is not a valid format");

        public static Error PasswordInvalid() => new Error(ErrorLevel.Critical,
                                                           ErrorType.Validation,
                                                           "The password needs to be atleast 12 characters long and contain one character, number and symbol");

        public static Error ConfirmPasswordInvalid() => new Error(ErrorLevel.Critical,
                                                                  ErrorType.Validation,
                                                                  "Both password and confirm password need to be the same");

        public static Error OneTimePasswordNotProvided() => new Error(ErrorLevel.Critical,
                                                                      ErrorType.Validation,
                                                                      "One time password was not provided");

        public static Error CredentialsNotProvided() => new Error(ErrorLevel.Critical,
                                                                  ErrorType.Validation,
                                                                  "Credentials were not provided");

        public static Error RegistrationNotProvided() => new Error(ErrorLevel.Critical,
                                                                   ErrorType.Validation,
                                                                   "Registration information was not provided");

        public static Error ChangePasswordNotProvided() => new Error(ErrorLevel.Critical,
                                                                     ErrorType.Validation,
                                                                     "Password change information was not provided");
    }
}
