using System;
using BurstChat.Shared.Errors;

namespace BurstChat.Api.Errors
{
    /// <summary>
    ///   This class exposes static methods that return errors associated with execution errors of the
    ///   application about private group message data.
    /// </summary>
    public static class PrivateGroupMessageErrors
    {
        public static Error GroupNotFound() => new Error(ErrorLevel.Critical,
                                                         ErrorType.DataProcess,
                                                         "The group does not exist");

        public static Error GroupMessageNotFound() => new Error(ErrorLevel.Critical,
                                                                ErrorType.DataProcess,
                                                                "The group message does not exist");
    }
}
