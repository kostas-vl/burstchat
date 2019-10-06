using System;
using System.Collections.Generic;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Api.Services.DirectMessagingService
{
    /// <summary>
    ///     This interface exposes methods for storing, fetching and transforming direct messages.
    /// </summary>
    public interface IDirectMessagingService
    {
        /// <summary>
        ///   This method will fetch all information about direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messages</param>
        /// <returns>An either monad</returns>
        Either<DirectMessaging, Error> Get(long userId, long directMessagingId);

        /// <summary>
        ///   This method will create a new direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the second participant</param>
        /// <returns>An either monad</returns>
        Either<DirectMessaging, Error> Insert(long userId, long firstParticipantId, long secondParticipantId);

        /// <summary>
        ///   This method will delete a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the group</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Delete(long userId, long directMessagingId);

        /// <summary>
        ///   This method will fetch all available messages of a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <returns>An either monad</returns>
        Either<IEnumerable<Message>, Error> GetMessages(long userId, long directMessagingId);

        /// <summary>
        ///   This method will add a new message to a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the target direct messaging entry</param>
        /// <param name="message">The message instance that will be used for the insertion</param>
        /// <returns>An either monad</returns>
        Either<Message, Error> InsertMessage(long userId, long directMessagingId, Message message);

        /// <summary>
        ///   This method will be edit a message of a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message instance that will be used for the edit</param>
        /// <returns>An either monad<returns>
        Either<Message, Error> UpdateMessage(long userId, long directMessagingId, Message message);

        /// <summary>
        ///   This method will delete a message from the direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> DeleteMessage(long userId, long directMessagingId, long messageId);
    }
}
