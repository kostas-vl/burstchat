using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Errors;
using BurstChat.Application.Extensions;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.UserService;
using BurstChat.Domain.Extensions;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.PrivateGroupsService
{
    /// <summary>
    ///   This class is the base implementation of the IPrivateGroupsService.
    /// </summary>
    public class PrivateGroupsProvider : IPrivateGroupsService
    {
        private readonly ILogger<PrivateGroupsProvider> _logger;
        private readonly IBurstChatContext _burstChatContext;
        private readonly IUserService _userService;

        /// <summary>
        ///   Executes any neccessary start up code for the service.
        /// </summary>
        public PrivateGroupsProvider(
            ILogger<PrivateGroupsProvider> logger,
            IBurstChatContext burstChatContext,
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
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An either monad</returns>
        public Either<PrivateGroup, Error> Get(long userId, long groupId)
        {
            try
            {
                var privateGroup = _burstChatContext
                    .PrivateGroups
                    .Include(pgm => pgm.Users)
                    .FirstOrDefault(pgm => pgm.Id == groupId);

                var userInList = privateGroup
                    .Users
                    .Any(u => u.Id == userId);

                if (privateGroup is { } && userInList)
                    return new Success<PrivateGroup, Error>(privateGroup);
                else
                    return new Failure<PrivateGroup, Error>(PrivateGroupErrors.GroupNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<PrivateGroup, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will create a new private group for messages.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupName">The name of the group</param>
        /// <returns>An either monad</returns>
        public Either<PrivateGroup, Error> Insert(long userId, string groupName)
        {
            try
            {
                var privateGroup = _burstChatContext
                    .PrivateGroups
                    .FirstOrDefault(pgm => pgm.Name == groupName);

                if (privateGroup is null)
                {
                    var newPrivateGroup = new PrivateGroup
                    {
                        Name = groupName,
                        DateCreated = DateTime.Now,
                    };

                    var user = _burstChatContext
                        .Users
                        .FirstOrDefault(u => u.Id == userId);

                    if (user is { })
                        newPrivateGroup.Users.Add(user);

                    _burstChatContext
                        .PrivateGroups
                        .Add(newPrivateGroup);

                    _burstChatContext.SaveChanges();

                    return new Success<PrivateGroup, Error>(newPrivateGroup);
                }
                else
                    return new Failure<PrivateGroup, Error>(PrivateGroupErrors.GroupNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<PrivateGroup, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Delete(long userId, long groupId)
        {
            try
            {
                return Get(userId, groupId).Bind(privateGroup =>
                {
                    _burstChatContext
                        .PrivateGroups
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
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="newUserId">The id of the user to be added</param>
        /// <returns>The either monad</returns>
        public Either<PrivateGroup, Error> InsertUser(long userId, long groupId, long newUserId)
        {
            try
            {
                return Get(userId, groupId).Bind<PrivateGroup>(privateGroup =>
                {
                    var user = _burstChatContext
                        .Users
                        .FirstOrDefault(u => u.Id == newUserId);

                    if (user is { })
                    {
                        privateGroup
                            .Users
                            .Add(user);

                        _burstChatContext.SaveChanges();

                        return new Success<PrivateGroup, Error>(privateGroup);
                    }
                    else
                        return new Failure<PrivateGroup, Error>(UserErrors.UserNotFound());
               });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<PrivateGroup, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will add a new user to a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userIds">The ids of the users to be added</param>
        /// <returns>The either monad</returns>
        public Either<PrivateGroup, Error> InsertUsers(long userId, long groupId, IEnumerable<long> userIds)
        {
            try
            {
                return Get(userId, groupId).Bind(privateGroup =>
                {
                    var idsToBeAdded = privateGroup
                        .Users
                        .Where(u => !userIds.Contains(u.Id));

                    var users = _burstChatContext
                        .Users
                        .AsEnumerable()
                        .Where(u => userIds.Contains(u.Id))
                        .ToList();

                    privateGroup
                        .Users
                        .AddRange(users);

                    _burstChatContext.SaveChanges();

                    return new Success<PrivateGroup, Error>(privateGroup);
                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<PrivateGroup, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will remove a user from a private group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="targetUserId">The id of the user that will be deleted</param>
        /// <returns>An either monad</returns>
        public Either<PrivateGroup, Error> DeleteUser(long userId, long groupId, long targetUserId)
        {
            try
            {
                return Get(userId, groupId).Bind<PrivateGroup>(privateGroup =>
                {
                    var targetUser = _burstChatContext
                        .Users
                        .FirstOrDefault(u => u.Id == targetUserId);

                    if (targetUser is { })
                    {
                        privateGroup
                            .Users
                            .Remove(targetUser);

                        _burstChatContext.SaveChanges();

                        return new Success<PrivateGroup, Error>(privateGroup);
                    }
                    else
                        return new Failure<PrivateGroup, Error>(UserErrors.UserNotFound());
                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<PrivateGroup, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will fetch all available messages of a group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Message>, Error> GetMessages(long userId, long groupId)
        {
            try
            {
                var privateGroup = _burstChatContext
                    .PrivateGroups
                    .Include(pgm => pgm.Messages)
                    .FirstOrDefault(pgm => pgm.Id == groupId);

                if (privateGroup is { })
                {
                    var userInList = privateGroup
                        .Users
                        .Any(u => u.Id == userId);

                    if (userInList)
                        return new Success<IEnumerable<Message>, Error>(privateGroup.Messages);
                }

                return new Failure<IEnumerable<Message>, Error>(PrivateGroupErrors.GroupNotFound());
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
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the target private group</param>
        /// <param name="message">The message instance that will be used for the insertion</param>
        /// <returns>An either monad</returns>
        public Either<Message, Error> InsertMessage(long userId, long groupId, Message message)
        {
            try
            {
                return Get(userId, groupId).Bind<Message>(group =>
                {
                    var user = _burstChatContext
                        .Users
                        .FirstOrDefault(u => u.Id == userId);

                    if (user is { } && message is { })
                    {
                        var newMessage = new Message
                        {
                            User = user,
                            Content = message.Content,
                            Edited = false,
                            DatePosted = DateTime.Now
                        };

                        group.Messages
                             .Add(newMessage);

                        _burstChatContext.SaveChanges();

                        return new Success<Message, Error>(newMessage);
                    }
                    else
                        return new Failure<Message, Error>(PrivateGroupErrors.GroupNotFound());
                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will be edit a message of a group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message instance that will be used for the edit</param>
        /// <returns>An either monad<returns>
        public Either<Message, Error> UpdateMessage(long userId, long groupId, Message message)
        {
            try
            {
                return GetMessages(userId, groupId).Bind<Message>(groupMessages =>
                {
                    var storedMessage = groupMessages
                        .FirstOrDefault(m => m.Id == message.Id);

                    if (storedMessage is { } && message is { })
                    {
                        storedMessage.Links = message.GetLinksFromContent();
                        storedMessage.Content = message.RemoveLinksFromContent();
                        storedMessage.Edited = true;

                        _burstChatContext.SaveChanges();

                        return new Success<Message, Error>(storedMessage);
                    }
                    else
                        return new Failure<Message, Error>(PrivateGroupErrors.GroupMessageNotFound());
                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete a message from the group.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> DeleteMessage(long userId, long groupId, long messageId)
        {
            try
            {
                return GetMessages(userId, groupId).Bind<Unit>(groupMessages =>
                {
                    var storedMessage = groupMessages
                        .FirstOrDefault(m => m.Id == messageId);

                    if (storedMessage is { })
                    {
                        _burstChatContext
                            .Messages
                            .Remove(storedMessage);

                        _burstChatContext.SaveChanges();

                        return new Success<Unit, Error>(new Unit());
                    }
                    else
                        return new Failure<Unit, Error>(PrivateGroupErrors.GroupMessageNotFound());
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
