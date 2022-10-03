using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Application.Errors;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Monads;
using BurstChat.Application.Extensions;
using BurstChat.Application.Services.ServersService;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.ChannelsService
{
    /// <summary>
    /// This class is a base implementation of the IChannelsService interface.
    /// </summary>
    public class ChannelsProvider : IChannelsService
    {
        private readonly ILogger<ChannelsProvider> _logger;
        private readonly IBurstChatContext _burstChatContext;
        private readonly IServersService _serversService;

        /// <summary>
        /// Executes any neccessary start up code for the controller.
        ///
        /// Exceptions:
        ///     ArgumentNullException: When any of the parameters is null.
        /// </summary>
        public ChannelsProvider(
            ILogger<ChannelsProvider> logger,
            IBurstChatContext burstChatContext,
            IServersService serversService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _burstChatContext = burstChatContext ?? throw new ArgumentNullException(nameof(burstChatContext));
            _serversService = serversService ?? throw new ArgumentNullException(nameof(serversService));
        }

        /// <summary>
        /// This method will return the instance of a server based on the provided user and channel id.
        /// </summary>
        /// <param name="userId">The id of the target user</param>
        /// <param name="channelId">the id of the target channel</param>
        /// <returns>An instance of a Server or null</returns>
        private Either<Server, Error> GetServer(long userId, int channelId)
        {
            try
            {
                var server = _burstChatContext
                    .Servers
                    .Include(s => s.Channels)
                    .Include(s => s.Subscriptions)
                    .AsQueryable()
                    .FirstOrDefault(s => s.Subscriptions.Any(sub => sub.UserId == userId)
                                            && s.Channels.Any(c => c.Id == channelId));

                if (server is null)
                    return new Failure<Server, Error>(ChannelErrors.ChannelNotFound());
                else
                    return new Success<Server, Error>(server);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Server, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will return the data of a channel based on the provided channel id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>An either monad</returns>
        public Either<Channel, Error> Get(long userId, int channelId)
        {
            try
            {
                return GetServer(userId, channelId).Bind<Channel>(_ =>
                {
                    var channel = _burstChatContext
                        .Channels
                        .FirstOrDefault(c => c.Id == channelId);

                    if (channel is not null && channel.IsPublic)
                        return new Success<Channel, Error>(channel);
                    else
                        return new Failure<Channel, Error>(ChannelErrors.ChannelNotFound());
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will insert a new channel and associate it with a server based
        /// on the provided parameters.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server that the channel will be associated with</param>
        /// <param name="channel">The channel that will be created<param>
        /// <returns>An either monad</returns>
        public Either<Channel, Error> Insert(long userId, int serverId, Channel channel)
        {
            try
            {
                return _serversService.Get(userId, serverId).Bind<Channel>(server =>
                {
                    var channelEntry = new Channel
                    {
                        Name = channel.Name,
                        IsPublic = channel.IsPublic,
                        DateCreated = DateTime.UtcNow
                    };
                    server
                        .Channels
                        .Add(channelEntry);
                    _burstChatContext.SaveChanges();

                    return new Success<Channel, Error>(channelEntry);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will update the information of a channel based on the provided parameters.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channel">The channel instance to be updated</param>
        /// <param name="channelDetails">The channel details to be updated</param>
        /// <returns>An either monad</returns>
        public Either<Channel, Error> Update(long userId, Channel channel)
        {
            try
            {
                if (channel is null)
                    return new Failure<Channel, Error>(ChannelErrors.ChannelNotFound());

                return Get(userId, channel.Id).Bind(channelEntry =>
                {
                    channelEntry.Name = channel.Name;
                    channelEntry.IsPublic = channel.IsPublic;
                    _burstChatContext.SaveChanges();

                    return new Success<Channel, Error>(channelEntry);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will remove a channel from the database based on the provided parameters.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channelId">The id channel to be deleted</param>
        /// <returns>An either monad</returns>
        public Either<Channel, Error> Delete(long userId, int channelId)
        {
            try
            {
                return Get(userId, channelId).Bind(channel =>
                {
                    _burstChatContext
                        .Channels
                        .Remove(channel);
                    _burstChatContext.SaveChanges();

                    return new Success<Channel, Error>(channel);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will return all messages posted in a channel based on the provided channel id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="searchTerm">A specific term that needs to exist in all returned messages</param>
        /// <param name="lastMessageId">The message id to be the interval of the message list</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Message>, Error> GetMessages(long userId,
                                                               int channelId,
                                                               string? searchTerm = null,
                                                               long? lastMessageId = null)
        {
            try
            {
                return GetServer(userId, channelId).Bind(_ =>
                {
                    var channel = _burstChatContext
                        .Channels
                        .First(c => c.Id == channelId);

                    var messages = _burstChatContext
                        .Channels
                        .Include(c => c.Messages)
                        .ThenInclude(m => m.User)
                        .Include(c => c.Messages)
                        .ThenInclude(m => m.Links)
                        .Where(c => c.Id == channelId)
                        .Select(c => c.Messages
                                      .Where(m => m.Id < (lastMessageId ?? long.MaxValue)
                                                  && (searchTerm == null || m.Content.Contains(searchTerm)))
                                      .OrderByDescending(m => m.Id)
                                      .Take(100))
                        .ToList()
                        .SelectMany(_ => _)
                        .OrderBy(m => m.Id)
                        .ToList();

                    return new Success<IEnumerable<Message>, Error>(messages);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<Message>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will insert a new message sent to the channel provided.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channelId">The id of the channel to which the message will be inserted</param>
        /// <param name="message">The message to be inserted</param>
        /// <returns>An either monad</returns>
        public Either<Message, Error> InsertMessage(long userId, int channelId, Message message)
        {
            try
            {
                return Get(userId, channelId).Bind<Message>(channel =>
                {
                    var user = _burstChatContext
                        .Users
                        .FirstOrDefault(u => u.Id == userId);

                    if (user is null || message is null)
                        return new Failure<Message, Error>(ChannelErrors.ChannelNotFound());

                    var newMessage = new Message
                    {
                        User = user,
                        Links = message.GetLinksFromContent(),
                        Content = message.RemoveLinksFromContent(),
                        Edited = false,
                        DatePosted = DateTime.UtcNow
                    };
                    channel.Messages.Add(newMessage);
                    _burstChatContext.SaveChanges();

                    return new Success<Message, Error>(newMessage);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will update the contents of the provided message on the provided channel.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channelId">The id of the channel of which the message will be updated</param>
        /// <param name="message">The message that will be updated</param>
        /// <returns>An either monad</returns>
        public Either<Message, Error> UpdateMessage(long userId, int channelId, Message message)
        {
            try
            {
                if (message is null)
                    return new Failure<Message, Error>(ChannelErrors.ChannelMessageNotFound());

                return Get(userId, channelId).Bind<Message>(_ =>
                {
                    var entries = _burstChatContext
                        .Channels
                        .Include(c => c.Messages)
                        .ThenInclude(m => m.User)
                        .Include(c => c.Messages)
                        .ThenInclude(m => m.Links)
                        .Where(c => c.Id == channelId)
                        .Select(c => c.Messages.FirstOrDefault(m => m.Id == message.Id))
                        .ToList();

                    if (entries.Count != 1)
                        return new Failure<Message, Error>(ChannelErrors.ChannelMessageNotFound());

                    var entry = entries.First()!;
                    entry.Links = message.GetLinksFromContent();
                    entry.Content = message.RemoveLinksFromContent();
                    entry.Edited = true;
                    _burstChatContext.SaveChanges();

                    return new Success<Message, Error>(entry);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will remove an existing message from a channel based on the provided id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An either monad</returns>
        public Either<Message, Error> DeleteMessage(long userId, int channelId, long messageId)
        {
            try
            {
                return Get(userId, channelId).Bind<Message>(_ =>
                {
                    var channel = _burstChatContext
                        .Channels
                        .Include(c => c.Messages.Where(m => m.Id == messageId))
                        .First(c => c.Id == channelId);

                    if (!channel.Messages.Any())
                        return new Failure<Message, Error>(ChannelErrors.ChannelMessageNotFound());

                    var message = channel.Messages.First();
                    channel.Messages.Remove(message);
                    _burstChatContext.SaveChanges();

                    return new Success<Message, Error>(message);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }
    }
}
