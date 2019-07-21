using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;

namespace BurstChat.Signal.Services.PrivateGroupMessaging
{
    /// <summary>
    ///   This interface exposes methods for data about messages.
    /// </summary>
    public interface IPrivateGroupMessagingService
    {
        /// <summary>
        ///   This method will fetch all messages of a private group based on the provided id.
        /// </summary>
        /// <param name="groupId">The id of the private group</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<IEnumerable<Message>, Error>> GetAllAsync(long groupId);

        /// <summary>
        ///   This method will post a new message to a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> PostAsync(long groupId, Message message);

        /// <summary>
        ///   This method will edit an existing message of a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> PutAsync(long groupId, Message message);

        /// <summary>
        ///   This method will delete an existing message of a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> DeleteAsync(long groupId, Message message);
    }
}
