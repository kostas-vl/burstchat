using System;
using System.Collections.Generic;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Api.Services.PrivateGroupMessaging
{
    public interface IPrivateGroupMessagingService
    {
        /// <summary>
        ///   This method will fetch all information about a private group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An either monad</returns>
        Either<PrivateGroupMessage, Error> Get(long groupId);

        /// <summary>
        ///   This method will create a new private group for messages.
        /// </summary>
        /// <param name="groupName">The name of the group</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Insert(string groupName);

        /// <summary>
        ///   This method will delete a private group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Delete(long groupId);

        /// <summary>
        ///   This method will add a new user to a private group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userId">The id of the user to be added</param>
        /// <returns>The either monad</returns>
        Either<Unit, Error> InsertUser(long groupId, long userId);

        /// <summary>
        ///   This method will remove a user from a private group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="userId">The id of the user that will be deleted</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> DeleteUser(long groupId, long userId);

        /// <summary>
        ///   This method will fetch all available messages of a group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <returns>An either monad</returns>
        Either<IEnumerable<Message>, Error> GetMessages(long groupId);

        /// <summary>
        ///   This method will add a new message to a private group.
        /// </summary>
        /// <param name="groupId">The id of the target private group</param>
        /// <param name="message">The message instance that will be used for the insertion</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> InsertMessage(long groupId, Message message);

        /// <summary>
        ///   This method will be edit a message of a group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message instance that will be used for the edit</param>
        /// <returns>An either monad<returns>
        Either<Unit, Error> UpdateMessage(long groupId, Message message);

        /// <summary>
        ///   This method will delete a message from the group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> DeleteMessage(long groupId, long messageId);
    }
}
