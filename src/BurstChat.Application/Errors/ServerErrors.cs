using System;

namespace BurstChat.Application.Errors
{
    /// <summary>
    /// This class contains static methods that return Error instance of operation that are related to server data that
    /// did not execute as expected.
    /// </summary>
    public static class ServerErrors
    {
        public static Error ServerNotFound() => new(ErrorLevel.Critical,
                                                    ErrorType.DataProcess,
                                                    "The server was not found");

        public static Error ServerAlreadyExists() => new(ErrorLevel.Critical,
                                                         ErrorType.Validation,
                                                         "A server with the same name already exists");

        public static Error UserAlreadyMember() => new(ErrorLevel.Critical,
                                                       ErrorType.DataProcess,
                                                       "The user is already a member of the server");
    }
}
