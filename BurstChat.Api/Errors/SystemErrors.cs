using System;
using BurstChat.Shared.Errors;

namespace BurstChat.Api.Errors
{
    /// <summary>
    /// A collection of static methods that represent errors occured that are related to dotnet or
    /// the runtime in general.
    /// </summary>
    public static class SystemErrors
    {
        /// <summary>
        /// This error can inform that an operation was not completed due to an unexpected exception.
        /// </summary>
        public static Error Exception() => new Error(ErrorLevel.Critical, 
                                                     ErrorType.System,
                                                     "An exception occured. Check the application logger for more details.");
    }
}
