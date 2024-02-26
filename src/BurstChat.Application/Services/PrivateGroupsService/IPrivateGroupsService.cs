using System.Collections.Generic;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;

namespace BurstChat.Application.Services.PrivateGroupsService
{
    public interface IPrivateGroupsService
    {
        /// <summary>
        /// This method will fetch all information about a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An either monad</returns>
        Result<PrivateGroup> Get(long userId, long groupId);

        /// <summary>
        /// This method will create a new private group for messages.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupName">The name of the group</param>
        /// <returns>An either monad</returns>
        Result<PrivateGroup> Insert(long userId, string groupName);

        /// <summary>
        /// This method will delete a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <returns>An either monad</returns>
        Result<Unit> Delete(long userId, long groupId);

        /// <summary>
        /// This method will add a new user to a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="newUserId">The id of the user to be added</param>
        /// <returns>The either monad</returns>
        Result<PrivateGroup> InsertUser(long userId, long groupId, long newUserId);

        /// <summary>
        /// This method will add a new user to a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userIds">The ids of the users to be added</param>
        /// <returns>The either monad</returns>
        Result<PrivateGroup> InsertUsers(long userI, long groupId, IEnumerable<long> userIds);

        /// <summary>
        /// This method will remove a user from a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="targetUserId">The id of the user that will be deleted</param>
        /// <returns>An either monad</returns>
        Result<PrivateGroup> DeleteUser(long userId, long groupId, long targetUserId);

        /// <summary>
        /// This method will fetch all available messages of a group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <returns>An either monad</returns>
        Result<IEnumerable<Message>> GetMessages(long userId, long groupId);

        /// <summary>
        /// This method will add a new message to a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the target private group</param>
        /// <param name="message">The message instance that will be used for the insertion</param>
        /// <returns>An either monad</returns>
        Result<Message> InsertMessage(long userId, long groupId, Message message);

        /// <summary>
        /// This method will be edit a message of a group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message instance that will be used for the edit</param>
        /// <returns>An either monad<returns>
        Result<Message> UpdateMessage(long userId, long groupId, Message message);

        /// <summary>
        /// This method will delete a message from the group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An either monad</returns>
        Result<Unit> DeleteMessage(long userId, long groupId, long messageId);
    }
}
