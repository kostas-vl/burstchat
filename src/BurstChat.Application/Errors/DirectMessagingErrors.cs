using System;

namespace BurstChat.Application.Errors
{
    /// <summary>
    /// This class exposes static methods that return errors associated with the execution errors of the
    /// application about direct messaging data.
    /// </summary>
    public static class DirectMessagingErrors
    {
        public static Error DirectMessagingNotFound() => new(ErrorLevel.Critical,
                                                             ErrorType.DataProcess,
                                                             "The direct messages dont exist");

        public static Error DirectMessagingAlreadyExists() => new(ErrorLevel.Critical,
                                                                  ErrorType.DataProcess,
                                                                  "The direct messages have already been associated");

        public static Error DirectMessagesNotFound() => new(ErrorLevel.Critical,
                                                            ErrorType.DataProcess,
                                                            "The direct messages were not found");

        public static Error DirectMessageNotFound() => new(ErrorLevel.Critical,
                                                           ErrorType.DataProcess,
                                                           "The direct message was not found");
    }
}
