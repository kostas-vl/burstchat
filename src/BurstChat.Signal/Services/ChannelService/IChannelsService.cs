using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Signal.Services.ChannelsService
{
    /// <summary>
    /// This interface exposes methods for fetching and manipulating channel messages
    /// </summary>
    public interface IChannelsService
    {
        /// <summary>
        /// This method will fetch information about a channel based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Channel, Error>> GetAsync(HttpContext context, int channelId);

        /// <summary>
        /// This method will invoke a call to the BurstChat API for the creation of a new channel.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="serverId">The id of the channel</param>
        /// <param name="channel">The instance of the new channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Channel, Error>> PostAsync(HttpContext context, int serverId, Channel channel);

        /// <summary>
        /// This method will invoke a call to the BurstChat API inorder to update information about
        /// a channel.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channel">The updated information of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Channel, Error>> PutAsync(HttpContext context, Channel channel);

        /// <summary>
        /// This method will invoke a call to the BurstChat API for the deletion of a channel.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Channel, Error>> DeleteAsync(HttpContext context, int channelId);

        /// <summary>
        /// This method will fetch all messages of a channels based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="searchTerm">A term that needs to be present on all returned messages</param>
        /// <param name="lastMessageId">The id of the interval message for the rest</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<IEnumerable<Message>, Error>> GetMessagesAsync(HttpContext context,
                                                                   int channelId,
                                                                   string? searchTerm = null,
                                                                   long? lastMessageId = null);

        /// <summary>
        /// This method will post a new message to a channel based on the provided channel id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Message, Error>> PostMessageAsync(HttpContext context, int channelId, Message message);

        /// <summary>
        /// This method will edit an existing message of a channel based on the provided channel id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Message, Error>> PutMessageAsync(HttpContext context, int channelId, Message message);

        /// <summary>
        /// This method will delete an existing message from a channel based on the provided channel id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Message, Error>> DeleteMessageAsync(HttpContext context, int channelId, Message message);
    }
}
