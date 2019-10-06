using System;
using BurstChat.Shared.Errors;

namespace BurstChat.Api.Errors
{
    /// <summary>
    ///   This class exposes static methods that return errors associated with the execution errors of the
    ///   application about direct messaging data.
    /// </summary>
    public static class DirectMessagingErrors
    {
        public static Error DirectMessagingNotFound() => new Error(ErrorLevel.Critical,
                                                                   ErrorType.DataProcess,
                                                                   "The direct messages dont exist");

        public static Error DirectMessagingAlreadyExists() => new Error(ErrorLevel.Critical,
                                                                        ErrorType.DataProcess,
                                                                        "The direct messages have already been associated");

        public static Error DirectMessagesNotFound() => new Error(ErrorLevel.Critical,
                                                                  ErrorType.DataProcess,
                                                                  "The direct messages were not found");
    }
}
