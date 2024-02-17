using BurstChat.Application.Monads;
using Level = BurstChat.Application.Monads.ErrorLevel;
using Type = BurstChat.Application.Monads.ErrorType;

namespace BurstChat.Application.Errors
{
    /// <summary>
    /// This class contains static methods that return Error instance of operation that are related to server data that
    /// did not execute as expected.
    /// </summary>
    public static class ServerErrors
    {
        public static MonadException ServerNotFound =>
            new MonadException(Level.Critical, Type.DataProcess, "The server was not found");

        public static MonadException ServerAlreadyExists =>
            new MonadException(Level.Critical, Type.Validation, "A server with the same name already exists");

        public static MonadException UserAlreadyMember =>
            new MonadException(Level.Critical, Type.DataProcess, "The user is already a member of the server");
    }
}
