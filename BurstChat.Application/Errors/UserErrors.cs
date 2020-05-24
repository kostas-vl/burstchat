using System;

namespace BurstChat.Application.Errors
{
    /// <summary>
    ///   This class contains static methods that represent errors associated with user errors when the
    ///   application tries to either fetch or transform data.
    /// </summary>
    public static class UserErrors
    {
        public static Error UserNotFound() => new Error(ErrorLevel.Critical,
                                                        ErrorType.Validation,
                                                        "The user was not found");

        public static Error UserAlreadyExists() => new Error(ErrorLevel.Critical,
                                                             ErrorType.Validation,
                                                             "The email is already registered");

        public static Error UserPasswordDidNotMatch() => new Error(ErrorLevel.Critical,
                                                                   ErrorType.Validation,
                                                                   "The email or password is incorrect");

        public static Error UserOneTimePasswordInvalid() => new Error(ErrorLevel.Critical,
                                                                      ErrorType.Validation,
                                                                      "The one time password is invalid");

        public static Error UserOneTimePasswordExpired() => new Error(ErrorLevel.Critical,
                                                                      ErrorType.Validation,
                                                                      "The one time password has expired");

        public static Error CouldNotUpdateInvitation() => new Error(ErrorLevel.Critical,
                                                                    ErrorType.DataProcess,
                                                                    "Could not update invitation");
    }
}
