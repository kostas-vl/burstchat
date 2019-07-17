using System;
using BurstChat.Shared.Errors;

namespace BurstChat.Api.Errors
{
    /// <summary>
    ///   This class contains static methods that represent errors associated with api model errors when the 
    ///   application tries to either fetch or transform data.
    /// </summary>
    public static class ModelErrors
    {
        public static Error EmailInvalid() => new Error(ErrorLevel.Critical,
                                                        ErrorType.Validation,
                                                        "The email is not a valid format");

        public static Error PasswordInvalid() => new Error(ErrorLevel.Critical,
                                                           ErrorType.Validation,
                                                           "The password needs to be atleast 12 characters long and contain one character, number and symbol");

        public static Error CredentialsNotProvided() => new Error(ErrorLevel.Critical,
                                                                  ErrorType.Validation,
                                                                  "Credentials were not provided");
    }
}
