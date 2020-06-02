using System;

namespace BurstChat.Application.Errors
{
    /// <summary>
    ///   This class contains static methods that represent errors associated with channel errors when the
    ///   application tries to either fetch or transform data.
    /// </summary>
    public static class ChannelErrors
    {
        public static Error ChannelNotFound() => new Error(ErrorLevel.Critical,
                                                           ErrorType.DataProcess,
                                                           "The channel was not found");

        public static Error ChannelMessageNotFound() => new Error(ErrorLevel.Critical,
                                                                  ErrorType.Validation,
                                                                  "The channel message was not found");
    }
}
