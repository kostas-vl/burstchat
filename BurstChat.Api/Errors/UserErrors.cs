using System;
using BurstChat.Shared.Errors;

namespace BurstChat.Api.Errors
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
    }
}
