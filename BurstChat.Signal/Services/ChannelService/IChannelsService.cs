using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;

namespace BurstChat.Signal.Services.ChannelsService
{
    /// <summary>
    ///   This interface exposes methods for fetching and manipulating channel messages
    /// </summary>
    public interface IChannelsService
    {
        /// <summary>
        ///   This method will fetch information about a channel based on the provided id.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Channel, Error>> GetChannelAsync(int channelId);

        /// <summary>
        ///   This method will fetch all messages of a channels based on the provided id.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<IEnumerable<Message>, Error>> GetAllAsync(int channelId);

        /// <summary>
        ///   This method will post a new message to a channel based on the provided channel id
        ///   and message.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> PostAsync(int channelId, Message message);

        /// <summary>
        ///  This method will edit an existing message of a channel based on the provided channel id 
        ///  and message.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> PutAsync(int channelId, Message message);

        /// <summary>
        ///   This method will delete an existing message from a channel based on the provided channel id
        ///   and message.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        Task<Either<Unit, Error>> DeleteAsync(int channelId, Message message);
    }
}
