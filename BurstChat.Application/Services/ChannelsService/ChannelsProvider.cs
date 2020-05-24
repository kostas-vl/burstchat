using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Application.Errors;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Monads;
using BurstChat.Application.Extensions;
using BurstChat.Application.Services.ServersService;
using BurstChat.Application.Services.UserService;
using BurstChat.Domain.Extensions;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using UserErrors = BurstChat.Application.Errors.UserErrors;

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
        private readonly IUserService _userService;

        /// <summary>
        /// Executes any neccessary start up code for the controller.
        /// </summary>
        public ChannelsProvider(
            ILogger<ChannelsProvider> logger,
            IBurstChatContext burstChatContext,
            IServersService serversService,
            IUserService userService
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
            _serversService = serversService;
            _userService = userService;
        }

        /// <summary>
        /// This method will return data of a channel based on the provided channel id but will contain
        /// also a batch of messages based on the last message id provided.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="lastMessageId">The message id to be the interval of the message list</param>
        /// <returns>An either monad</returns>
        public Either<Channel, Error> GetWithMessages(long userId, int channelId, long? lastMessageId = null)
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

                if (server is { })
                {
                    var channel = _burstChatContext
                        .Channels
                        .First(c => c.Id == channelId);

                    channel.Messages = _burstChatContext
                        .Channels
                        .Include(c => c.Messages)
                        .ThenInclude(m => m.User)
                        .Include(c => c.Messages)
                        .ThenInclude(m => m.Links)
                        .Where(c => c.Id == channelId)
                        .Select(c => c.Messages
                                      .Where(m => m.Id < (lastMessageId ?? long.MaxValue))
                                      .OrderByDescending(m => m.Id)
                                      .Take(100))
                        .ToList()
                        .Aggregate(new List<Message>(), (current, next) =>
                        {
                            current.AddRange(next);
                            return current;
                        })
                        .OrderBy(m => m.Id)
                        .ToList();

                    return new Success<Channel, Error>(channel);
                }

                return new Failure<Channel, Error>(ChannelErrors.ChannelNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Channel, Error>(SystemErrors.Exception());
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
                var server = _burstChatContext
                    .Servers
                    .Include(s => s.Channels)
                    .Include(s => s.Subscriptions)
                    .AsQueryable()
                    .FirstOrDefault(s => s.Subscriptions.Any(s => s.UserId == userId)
                                         && s.Channels.Any(c => c.Id == channelId));

                if (server is { })
                {
                    var channel = _burstChatContext
                        .Channels
                        .FirstOrDefault(c => c.Id == channelId);

                    if (channel is { } && channel.IsPublic)
                        return new Success<Channel, Error>(channel);
                }

                return new Failure<Channel, Error>(ChannelErrors.ChannelNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
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
                return _serversService
                    .Get(userId, serverId)
                    .Bind<Channel>(server =>
                    {
                        var userMonad = _userService.Get(userId);
                        if (userMonad is Failure<User, Error>)
                            return new Failure<Channel, Error>(UserErrors.UserNotFound());

                        var user = (userMonad as Success<User, Error>)!.Value;

                        var channelEntry = new Channel
                        {
                            Name = channel.Name,
                            IsPublic = channel.IsPublic,
                            DateCreated = DateTime.Now
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
                _logger.LogException(e);
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
                if (channel is { })
                {
                    var channelId = channel.Id;

                    return Get(userId, channelId).Bind(channelEntry =>
                    {
                        channelEntry.Name = channel.Name;
                        channelEntry.IsPublic = channel.IsPublic;

                        _burstChatContext.SaveChanges();

                        return new Success<Channel, Error>(channelEntry);
                    });
                }
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
                _logger.LogException(e);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will return all messages posted in a channel based on the provided channel id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="lastMessageId">The message id to be the interval of the message list</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Message>, Error> GetMessages(long userId, int channelId, long? lastMessageId = null)
        {
            return GetWithMessages(userId, channelId, lastMessageId).Attach(channel => channel.Messages as IEnumerable<Message>);
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

                    if (user is { } && message is { })
                    {
                        var newMessage = new Message
                        {
                            User = user,
                            Links = message.GetLinksFromContent(),
                            Content = message.RemoveLinksFromContent(),
                            Edited = false,
                            DatePosted = DateTime.Now
                        };

                        channel.Messages
                               .Add(newMessage);

                        _burstChatContext.SaveChanges();

                        return new Success<Message, Error>(newMessage);
                    }
                    else
                        return new Failure<Message, Error>(ChannelErrors.ChannelNotFound());
                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
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
                return GetWithMessages(userId, channelId, message.Id).Bind<Message>(channel =>
                {
                    var messageEntry = channel.Messages
                                              .FirstOrDefault(m => m.Id == message.Id);

                    if (messageEntry is { } && message is { })
                    {
                        messageEntry.Links = message.GetLinksFromContent();
                        messageEntry.Content = message.RemoveLinksFromContent();
                        messageEntry.Edited = true;

                        _burstChatContext.SaveChanges();

                        return new Success<Message, Error>(messageEntry);
                    }
                    else
                        return new Failure<Message, Error>(ChannelErrors.ChannelMessageNotFound());
                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
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
                return GetWithMessages(userId, channelId, messageId).Bind(channel =>
                {
                    var message = channel.Messages.First(m => m.Id == messageId);

                    channel.Messages.Remove(message);

                    _burstChatContext.SaveChanges();

                    return new Success<Message, Error>(message);
                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }
    }
}
