using BurstChat.Application.Monads;
using Level = BurstChat.Application.Monads.ErrorLevel;
using Type = BurstChat.Application.Monads.ErrorType;

namespace BurstChat.Application.Errors
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
        public static MonadException Exception =>
            new MonadException(
                Level.Critical,
                Type.System,
                "One or more actions could not be completed"
            );
    }
}
