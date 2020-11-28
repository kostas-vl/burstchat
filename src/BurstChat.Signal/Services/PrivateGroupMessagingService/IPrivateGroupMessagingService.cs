using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Signal.Services.PrivateGroupMessaging
{
    /// <summary>
    /// This interface exposes methods for data about messages.
    /// </summary>
    public interface IPrivateGroupMessagingService
    {
        /// <summary>
        /// This method will fetch information about a private group based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the private group</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<PrivateGroup, Error>> GetPrivateGroupAsync(HttpContext context, long groupId);

        /// <summary>
        /// This method will fetch all messages of a private group based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the private group</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<IEnumerable<Message>, Error>> GetAllAsync(HttpContext context, long groupId);

        /// <summary>
        /// This method will post a new message to a private group based on the provided group id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> PostAsync(HttpContext context, long groupId, Message message);

        /// <summary>
        /// This method will edit an existing message of a private group based on the provided group id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> PutAsync(HttpContext context, long groupId, Message message);

        /// <summary>
        /// This method will delete an existing message of a private group based on the provided group id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> DeleteAsync(HttpContext context, long groupId, Message message);
    }
}
