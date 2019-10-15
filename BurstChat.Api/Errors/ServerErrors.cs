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
        public static Error ServerNotFound() => new Error(ErrorLevel.Critical, 
                                                          ErrorType.DataProcess,
                                                          "The server was not found");

        public static Error ServerAlreadyExists() => new Error(ErrorLevel.Critical,
                                                               ErrorType.Validation,
                                                               "A server with the same name already exists");

        public static Error UserAlreadyMember() => new Error(ErrorLevel.Critical,
                                                             ErrorType.DataProcess,
                                                             "The user is already a member of the server");


    }
}
