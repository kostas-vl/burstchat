using System;

namespace BurstChat.Application.Errors
{
    /// <summary>
    ///   This class exposes static methods that return errors associated with execution errors of the
    ///   application about private group message data.
    /// </summary>
    public static class PrivateGroupErrors
    {
        public static Error GroupNotFound() => new Error(ErrorLevel.Critical,
                                                         ErrorType.DataProcess,
                                                         "The group does not exist");

        public static Error GroupMessageNotFound() => new Error(ErrorLevel.Critical,
                                                                ErrorType.DataProcess,
                                                                "The group message does not exist");
    }
}
