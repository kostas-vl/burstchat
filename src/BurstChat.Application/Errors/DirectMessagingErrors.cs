using BurstChat.Application.Monads;
using Level = BurstChat.Application.Monads.ErrorLevel;
using Type = BurstChat.Application.Monads.ErrorType;

namespace BurstChat.Application.Errors
{
    /// <summary>
    /// This class exposes static methods that return errors associated with the execution errors of the
    /// application about direct messaging data.
    /// </summary>
    public static class DirectMessagingErrors
    {
        public static MonadException DirectMessagingNotFound =>
            new MonadException(Level.Critical, Type.DataProcess, "The direct messages dont exist");

        public static MonadException DirectMessagingAlreadyExists =>
            new MonadException(Level.Critical, Type.DataProcess, "The direct messages have already been associated");

        public static MonadException DirectMessagesNotFound =>
            new MonadException(Level.Critical, Type.DataProcess, "The direct messages were not found");

        public static MonadException DirectMessageNotFound =>
            new MonadException(Level.Critical, Type.DataProcess, "The direct message was not found");
    }
}
