using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Api.Errors;
using BurstChat.Api.Services.ServersService;
using BurstChat.Shared.Context;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Services.ChannelsService
{
    /// <summary>
    ///   This class is a base implementation of the IChannelsService interface.
    /// </summary>
    public class ChannelsProvider : IChannelsService
    {
        private readonly ILogger<ChannelsProvider> _logger;
        private readonly BurstChatContext _burstChatContext;
        private readonly IServersService _serversService;

        /// <summary>
        ///   Executes any neccessary start up code for the controller.
        /// </summary>
        public ChannelsProvider(
            ILogger<ChannelsProvider> logger,
            BurstChatContext burstChatContext,
            IServersService serversService
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
            _serversService = serversService;
        }

        /// <summary>
        ///   This method will return the data of a channel based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>An either monad</returns>
        public Either<Channel, Error> Get(int channelId)
        {
            try
            {
                var channel = _burstChatContext
                    .Channels
                    .Include(c => c.Details)
                    .ThenInclude(d => d.Messages)
                    .FirstOrDefault(c => c.Id == channelId);

                if (channel != null)
                    return new Success<Channel, Error>(channel);
                else
                    return new Failure<Channel, Error>(ChannelErrors.ChannelNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will insert a new channel and associate it with a server based
        ///   on the provided parameters.
        /// </summary>
        /// <param name="serverId">The id of the server that the channel will be associated with</param>
        /// <param name="channel">The channel that will be created<param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Insert(int serverId, Channel channel)
        {
            try
            {
                return _serversService
                    .Get(serverId)
                    .Bind(server => 
                    {
                        var channelEntry = new Channel
                        {
                            Name = channel.Name,
                            IsPublic = channel.IsPublic,
                            DateCreated = DateTime.Now,
                            Details = new ChannelDetails()
                        };

                        server
                            .Channels
                            .Add(channelEntry);

                        _burstChatContext.SaveChanges();

                        return new Success<Unit, Error>(new Unit());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will update the information of a channel based on the provided parameters.
        /// </summary>
        /// <param name="channel">The channel instance to be updated</param>
        /// <param name="channelDetails">The channel details to be updated</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Update(Channel channel)
        {
            try
            {
                var channelId = channel?.Id ?? default(int);

                return Get(channelId)
                    .Bind(channelEntry => 
                    {
                        channelEntry.Name = channel.Name;
                        channelEntry.IsPublic = channel.IsPublic;

                        _burstChatContext.SaveChanges();

                        return new Success<Unit, Error>(new Unit());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will remove a channel from the database based on the provided parameters.
        /// </summary>
        /// <param name="channelId">The id channel to be deleted</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Delete(int channelId)
        {
            try
            {
                return Get(channelId)
                    .Bind(channel => 
                    {
                        _burstChatContext
                            .Channels
                            .Remove(channel);

                        _burstChatContext.SaveChanges();

                        return new Success<Unit, Error>(new Unit());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will return all messages posted in a channel based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Message>, Error> GetMessages(int channelId) =>
            Get(channelId)
                .Bind(channel => new Success<IEnumerable<Message>, Error>(channel.Details.Messages));

        /// <summary>
        ///   This method will insert a new message sent to the channel provided.
        /// </summary>
        /// <param name="channelId">The id of the channel to which the message will be inserted</param>
        /// <param name="message">The message to be inserted</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> InsertMessage(int channelId, Message message)
        {
            try
            {
                return Get(channelId)
                    .Bind(channel => 
                    {
                        channel
                            .Details
                            .Messages
                            .Add(message);

                        _burstChatContext.SaveChanges();

                        return new Success<Unit, Error>(new Unit());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will update the contents of the provided message on the provided channel.
        /// </summary>
        /// <param name="channelId">The id of the channel of which the message will be updated</param>
        /// <param name="message">The message that will be updated</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> UpdateMessage(int channelId, Message message)
        {
            try
            {
                return Get(channelId)
                    .Bind<Unit>(channel =>
                    {
                        var messageEntry = channel
                            .Details
                            .Messages
                            .FirstOrDefault(m => m.Id == message.Id);

                        if (messageEntry != null)
                        {
                            messageEntry.Content = message.Content;
                            messageEntry.Edited = true;

                            _burstChatContext.SaveChanges();

                            return new Success<Unit, Error>(new Unit());
                        }
                        else
                            return new Failure<Unit, Error>(ChannelErrors.ChannelMessageNotFound());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will remove an existing message from a channel based on the provided id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> DeleteMessage(int channelId, Message message)
        {
            try
            {
                return Get(channelId)
                    .Bind(channel => 
                    {
                        channel
                            .Details
                            .Messages
                            .Remove(message);

                        _burstChatContext.SaveChanges();

                        return new Success<Unit, Error>(new Unit());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }
    }
}
