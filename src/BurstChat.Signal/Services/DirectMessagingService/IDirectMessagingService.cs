using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Signal.Services.DirectMessagingService
{
    /// <summary>
    /// This interface exposes methods for interacting with the BurstChat API for direct messaging data.
    /// </summary>
    public interface IDirectMessagingService
    {
        /// <summary>
        /// Fetches all available information about a direct messaging entry based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the target entry</param>
        /// <returns>An either monad</returns>
        Task<Either<DirectMessaging, Error>> GetAsync(HttpContext context, long directMessagingId);

        /// <summary>
        /// Fetches all available information about a direct messaging entry based on the provided
        /// participants.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the seconad participant</param>
        /// <returns>An either monad</returns>
        Task<Either<DirectMessaging, Error>> GetAsync(HttpContext context, long firstParticipantId, long secondParticipantId);

        /// <summary>
        /// Creates a new direct messaging entry between two users based on the provided user ids.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessaging">The direct messaging instance to be added</param>
        /// <returns>An either monad</returns>
        Task<Either<DirectMessaging, Error>> PostAsync(HttpContext context, DirectMessaging directMessaging);

        /// <summary>
        /// Removes a direct messaging entry based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the target entry</param>
        /// <returns>An either monad</returns>
        Task<Either<DirectMessaging, Error>> DeleteAsync(HttpContext context, long directMessagingId);

        /// <summary>
        /// Fetches all messages posted on a direct messaging entry.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="lastMessageId">The message id from which all the previous messages sent will be fetched</param>
        /// <returns>An either monad</returns>
        Task<Either<IEnumerable<Message>, Error>> GetMessagesAsync(HttpContext context,
                                                                   long directMessagingId,
                                                                   long? lastMessageId = null);

        /// <summary>
        /// Inserts a new message on a direct messaging entry.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message to be inserted</param>
        /// <returns>An either monad</returns>
        Task<Either<Message, Error>> PostMessageAsync(HttpContext context, long directMessagingId, Message message);

        /// <summary>
        /// Updates a message on a direct messaging entry.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>An either monad</returns>
        Task<Either<Message, Error>> PutMessageAsync(HttpContext context, long directMessagingId, Message message);

        /// <summary>
        /// Removes a message from a direct messagin entry.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message to be removed</param>
        /// <returns>An either monad</returns>
        Task<Either<Message, Error>> DeleteMessageAsync(HttpContext context, long directMessagingId, Message message);
    }
}
