using System.Collections.Generic;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;

namespace BurstChat.Application.Services.DirectMessagingService
{
    /// <summary>
    /// This interface exposes methods for storing, fetching and transforming direct messages.
    /// </summary>
    public interface IDirectMessagingService
    {
        /// <summary>
        /// This method will fetch all information about direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messages</param>
        /// <returns>An either monad</returns>
        Result<DirectMessaging> Get(long userId, long directMessagingId);

        /// <summary>
        /// This method will fetch all information about direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the second participant</param>
        /// <returns>An either monad</returns>
        Result<DirectMessaging> Get(long userId, long firstParticipantId, long secondParticipantId);

        /// <summary>
        /// This method will fetch all users that the requesting user has direct messaged.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <returns>An either monad</returns>
        Result<IEnumerable<User?>> GetUsers(long userId);

        /// <summary>
        /// This method will create a new direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the second participant</param>
        /// <returns>An either monad</returns>
        Result<DirectMessaging> Insert(
            long userId,
            long firstParticipantId,
            long secondParticipantId
        );

        /// <summary>
        /// This method will delete a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the group</param>
        /// <returns>An either monad</returns>
        Result<DirectMessaging> Delete(long userId, long directMessagingId);

        /// <summary>
        /// This method will fetch all available messages of a direct messaging entry.
        /// If a message id is provided then 300 messages sent prior will be returned.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="searchTerm">A term that needs to be present in all returned messages</param>
        /// <param name="lastMessageId">The message id from which all prior messages will be fetched</param>
        /// <returns>An either monad</returns>
        Result<IEnumerable<Message>> GetMessages(
            long userId,
            long directMessagingId,
            string? searchTerm = null,
            long? lastMessageId = null
        );

        /// <summary>
        /// This method will add a new message to a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the target direct messaging entry</param>
        /// <param name="message">The message instance that will be used for the insertion</param>
        /// <returns>An either monad</returns>
        Result<Message> InsertMessage(long userId, long directMessagingId, Message message);

        /// <summary>
        /// This method will be edit a message of a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message instance that will be used for the edit</param>
        /// <returns>An either monad<returns>
        Result<Message> UpdateMessage(long userId, long directMessagingId, Message message);

        /// <summary>
        /// This method will delete a message from the direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An either monad</returns>
        Result<Message> DeleteMessage(long userId, long directMessagingId, long messageId);
    }
}
