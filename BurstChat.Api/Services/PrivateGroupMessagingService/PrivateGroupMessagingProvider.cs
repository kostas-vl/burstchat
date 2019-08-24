using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Api.Errors;
using BurstChat.Shared.Context;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Services.UserService;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Services.PrivateGroupMessaging
{
    /// <summary>
    ///   This class is the base implementation of the IPrivateGroupMessagingService.
    /// </summary>
    public class PrivateGroupMessagingProvider : IPrivateGroupMessagingService
    {
        private readonly ILogger<PrivateGroupMessagingProvider> _logger;
        private readonly BurstChatContext _burstChatContext;
        private readonly IUserService _userService;

        /// <summary>
        ///   Executes any neccessary start up code for the service.
        /// </summary>
        public PrivateGroupMessagingProvider(
            ILogger<PrivateGroupMessagingProvider> logger,
            BurstChatContext burstChatContext,
            IUserService userService
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
            _userService = userService;
        }

        /// <summary>
        ///   This method will fetch all information about a private group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An either monad</returns>
        public Either<PrivateGroupMessage, Error> Get(long groupId)
        {
            try
            {
                var privateGroup = _burstChatContext
                    .PrivateGroupMessage
                    .Include(pgm => pgm.Users)
                    .FirstOrDefault(pgm => pgm.Id == groupId);

                if (privateGroup != null)
                    return new Success<PrivateGroupMessage, Error>(privateGroup);
                else
                    return new Failure<PrivateGroupMessage, Error>(PrivateGroupMessageErrors.GroupNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<PrivateGroupMessage, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will create a new private group for messages.
        /// </summary>
        /// <param name="groupName">The name of the group</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Insert(string groupName)
        {
            try
            {
                var privateGroup = _burstChatContext
                    .PrivateGroupMessage
                    .FirstOrDefault(pgm => pgm.Name == groupName);
                
                if (privateGroup == null)
                {
                    var newPrivateGroup = new PrivateGroupMessage
                    {
                        Name = groupName,
                        DateCreated = DateTime.Now
                    };

                    _burstChatContext
                        .PrivateGroupMessage
                        .Add(newPrivateGroup);

                    _burstChatContext.SaveChanges();

                    return new Success<Unit, Error>(new Unit());
                }
                else
                    return new Failure<Unit, Error>(PrivateGroupMessageErrors.GroupNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete a private group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Delete(long groupId)
        {
            try
            {
                return Get(groupId)
                    .Bind(privateGroup =>
                    {
                        _burstChatContext
                            .PrivateGroupMessage
                            .Remove(privateGroup);

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
        ///   This method will add a new user to a private group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userId">The id of the user to be added</param>
        /// <returns>The either monad</returns>
        public Either<Unit, Error> InsertUser(long groupId, long userId)
        {
            try
            {
                return _userService
                    .Get(userId)
                    .Bind(user => Get(groupId)
                                      .Attach(privateGroup => (privateGroup, user)))
                    .Bind(privateGroupAndUser =>
                    {
                        var (privateGroup, user) = privateGroupAndUser;

                        privateGroup
                            .Users
                            .Add(user);

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
        ///   This method will remove a user from a private group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="userId">The id of the user that will be deleted</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> DeleteUser(long groupId, long userId)
        {
            try
            {
                return _userService
                    .Get(userId)
                    .Bind(user => Get(groupId)
                                      .Attach(privateGroup => (privateGroup, user)))
                    .Bind(privateGroupAndUser =>
                    {
                        var (privateGroup, user) = privateGroupAndUser;

                        privateGroup
                            .Users
                            .Remove(user);

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
        ///   This method will fetch all available messages of a group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Message>, Error> GetMessages(long groupId)
        {
            try
            {
                var privateGroup = _burstChatContext
                    .PrivateGroupMessage
                    .Include(pgm => pgm.Messages)
                    .FirstOrDefault(pgm => pgm.Id == groupId);

                if (privateGroup != null)
                    return new Success<IEnumerable<Message>, Error>(privateGroup.Messages);
                else
                    return new Failure<IEnumerable<Message>, Error>(PrivateGroupMessageErrors.GroupNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<IEnumerable<Message>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will add a new message to a private group.
        /// </summary>
        /// <param name="groupId">The id of the target private group</param>
        /// <param name="message">The message instance that will be used for the insertion</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> InsertMessage(long groupId, Message message)
        {
            try
            {
                return GetMessages(groupId)
                    .Bind(groupMessages =>
                    {
                        var groupMessagesList = groupMessages.ToList();
                        var newMessage = new Message
                        {
                            UserId = message.UserId,
                            Content = message.Content,
                            Edited = false,
                            DatePosted = DateTime.Now
                        };

                        groupMessagesList.Add(newMessage);
                            
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
        ///   This method will be edit a message of a group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message instance that will be used for the edit</param>
        /// <returns>An either monad<returns>
        public Either<Unit, Error> UpdateMessage(long groupId, Message message)
        {
            try
            {
                return GetMessages(groupId)
                    .Bind<Unit>(groupMessages =>
                    {
                        var storedMessage = groupMessages
                            .FirstOrDefault(m => m.Id == message.Id);

                        if (storedMessage != null)
                        {
                            storedMessage.Content = message.Content;
                            storedMessage.Edited = true;

                            _burstChatContext.SaveChanges();

                            return new Success<Unit, Error>(new Unit());
                        }
                        else
                            return new Failure<Unit, Error>(PrivateGroupMessageErrors.GroupMessageNotFound());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete a message from the group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> DeleteMessage(long groupId, long messageId)
        {
            try
            {
                return GetMessages(groupId)
                    .Bind<Unit>(groupMessages =>
                    {
                        var storedMessage = groupMessages
                            .FirstOrDefault(m => m.Id == messageId);

                        if (storedMessage != null)
                        {
                            _burstChatContext
                                .Messages
                                .Remove(storedMessage);

                            _burstChatContext.SaveChanges();

                            return new Success<Unit, Error>(new Unit());
                        }
                        else
                            return new Failure<Unit, Error>(PrivateGroupMessageErrors.GroupMessageNotFound());
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
