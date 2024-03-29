using System.Collections.Generic;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;

namespace BurstChat.Application.Services.ChannelsService;

/// <summary>
/// This interface exposes methods for interacting with data of chat channels.
/// </summary>
public interface IChannelsService
{
    /// <summary>
    /// This method will return the data of a channel based on the provided channel id.
    /// </summary>
    /// <param name="userId">The id of the requesting user</param>
    /// <param name="channelId">The id of the target channel</param>
    /// <returns>An either monad</returns>
    Result<Channel> Get(long userId, int channelId);

    /// <summary>
    /// This method will insert a new channel and associate it with a server based
    /// on the provided parameters.
    /// </summary>
    /// <param name="serverId">The id of the server that the channel will be associated with</param>
    /// <param name="channel">The channel that will be created<param>
    /// <returns>An either monad</returns>
    Result<Channel> Insert(long userId, int serverId, Channel channel);

    /// <summary>
    /// This method will update the information of a channel based on the provided parameters.
    /// </summary>
    /// <param name="userId">The id of the requesting user</param>
    /// <param name="channel">The channel instance to be updated</param>
    /// <returns>An either monad</returns>
    Result<Channel> Update(long userId, Channel channel);

    /// <summary>
    /// This method will remove a channel from the database based on the provided parameters.
    /// </summary>
    /// <param name="userId">The id of the requesting user</param>
    /// <param name="channelId">The id of the channel to be deleted</param>
    /// <returns>An either monad</returns>
    Result<Channel> Delete(long userId, int channelId);

    /// <summary>
    /// This method will return all messages posted in a channel based on the provided channel id.
    /// </summary>
    /// <param name="userId">The id of the requesting user</param>
    /// <param name="channelId">The id of the target channel</param>
    /// <param name="searchTerm">A specific term that needs to exist in all returned messages</param>
    /// <param name="lastMessageId">The message id to be the interval of the message list</param>
    /// <returns>An either monad</returns>
    Result<IEnumerable<Message>> GetMessages(
        long userId,
        int channelId,
        string? searchTerm = null,
        long? lastMessageId = null
    );

    /// <summary>
    /// This method will insert a new message sent to the channel provided.
    /// </summary>
    /// <param name="userId">The id of the requesting user</param>
    /// <param name="channelId">The id of the channel to which the message will be inserted</param>
    /// <param name="message">The message to be inserted</param>
    /// <returns>An either monad</returns>
    Result<Message> InsertMessage(long userId, int channelId, Message message);

    /// <summary>
    /// This method will update the contents of the provided message on the provided channel.
    /// </summary>
    /// <param name="userId">The id of the requesting user</param>
    /// <param name="channelId">The id of the channel of which the message will be updated</param>
    /// <param name="message">The message that will be updated</param>
    /// <returns>An either monad</returns>
    Result<Message> UpdateMessage(long userId, int channelId, Message message);

    /// <summary>
    /// This method will remove an existing message from a channel based on the provided id.
    /// </summary>
    /// <param name="userId">The id of the requesting user</param>
    /// <param name="channelId">The id of the target channel</param>
    /// <param name="messageId">The id of the message to be deleted</param>
    /// <returns>An either monad</returns>
    Result<Message> DeleteMessage(long userId, int channelId, long messageId);
}
