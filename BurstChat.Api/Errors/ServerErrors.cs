using System;
using BurstChat.Shared.Errors;

namespace BurstChat.Api.Errors
{
    /// <summary>
    ///   This class contains static methods that return Error instance of operation that are related to server data that
    ///   did not execute as expected.
    /// </summary>
    public static class ServerErrors
    {
        /// <summary>
        ///   This method infrorm the application that a server was not found based on an executed operation.
        /// </summary>
        public static Error ServerNotFound() => new Error(ErrorLevel.Critical, 
                                                          ErrorType.DataProcess,
                                                          "The server was not found");
    }
}
