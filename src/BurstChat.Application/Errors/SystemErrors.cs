using System;

namespace BurstChat.Application.Errors
{
    /// <summary>
    ///   A collection of static methods that represent errors occured that are related to dotnet or
    ///   the runtime in general.
    /// </summary>
    public static class SystemErrors
    {
        /// <summary>
        ///   This error can inform that an operation was not completed due to an unexpected exception.
        /// </summary>
        public static Error Exception() => new Error(ErrorLevel.Critical,
                                                     ErrorType.System,
                                                     "One or more actions could not be completed");
    }
}
