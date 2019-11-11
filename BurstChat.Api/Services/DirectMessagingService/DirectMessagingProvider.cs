using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Api.Errors;
using BurstChat.Api.Extensions;
using BurstChat.Shared.Context;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Services.DirectMessagingService
{
    /// <summary>
    ///     This class is the base implementation of the IDirectMessagingService.
    /// </summary>
    public class DirectMessagingProvider : IDirectMessagingService
    {
        private readonly ILogger<DirectMessagingProvider> _logger;
        private readonly BurstChatContext _burstChatContext;

        /// <summary>
        ///     Creates a new instance of DirectMessagingProvider.
        /// </summary>
        public DirectMessagingProvider(
            ILogger<DirectMessagingProvider> logger,
            BurstChatContext burstChatContext
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
        }

        /// <summary>
        ///   This method will fetch all information about direct messaging entry.
        ///   If a target date is provided then 300 messages sent prior will be returned.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messages</param>
        /// <param name="targetDate">The last date and time the messages where sent</param>
        /// <returns>An either monad</returns>
        private Either<DirectMessaging, Error> GetWithMessages(long userId, long directMessagingId, DateTime? targetDate = null)
        {
            try
            {
                var directMessaging = _burstChatContext
                    .DirectMessaging
                    .FirstOrDefault(dm => dm.Id == directMessagingId
                                          && (dm.FirstParticipantUserId == userId
                                              || dm.SecondParticipantUserId == userId));

                if (directMessaging is { })
                {
                    directMessaging.Messages = _burstChatContext
                        .DirectMessaging
                        .Include(dm => dm.Messages)
                        .ThenInclude(m => m.User)
                        .Where(dm => dm.Id == directMessagingId)
                        .Select(dm => dm.Messages
                                        .Where(m => m.DatePosted <= (targetDate ?? DateTime.Now))
                                        .OrderBy(m => m.DatePosted)
                                        .Take(300))                        
                        .ToList()
                        .Aggregate(new List<Message>(), (current, next) =>
                        {
                            current.AddRange(next);
                            return current;
                        });
                  
                    return new Success<DirectMessaging, Error>(directMessaging);
                }
                else
                    return new Failure<DirectMessaging, Error>(DirectMessagingErrors.DirectMessagingNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<DirectMessaging, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will fetch all information about direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messages</param>
        /// <returns>An either monad</returns>
        public Either<DirectMessaging, Error> Get(long userId, long directMessagingId)
        {
            try
            {
                var directMessaging = _burstChatContext
                    .DirectMessaging
                    .FirstOrDefault(dm => dm.Id == directMessagingId
                                          && (dm.FirstParticipantUserId == userId
                                              || dm.SecondParticipantUserId == userId));

                if (directMessaging is { })
                    return new Success<DirectMessaging, Error>(directMessaging);
                else
                    return new Failure<DirectMessaging, Error>(DirectMessagingErrors.DirectMessagingNotFound());

            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<DirectMessaging, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will fetch all information about direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the second participant</param>
        /// <returns>An either monad</returns>
        public Either<DirectMessaging, Error> Get(long userId, long firstParticipantId, long secondParticipantId)
        {
            try
            {
                if (userId != firstParticipantId && userId != secondParticipantId)
                    return new Failure<DirectMessaging, Error>(DirectMessagingErrors.DirectMessagingNotFound());

                var directMessaging = _burstChatContext
                    .DirectMessaging
                    .FirstOrDefault(dm => (dm.FirstParticipantUserId == firstParticipantId && dm.SecondParticipantUserId == secondParticipantId)
                                          || (dm.FirstParticipantUserId == secondParticipantId && dm.SecondParticipantUserId == firstParticipantId));

                if (directMessaging is { })
                    return new Success<DirectMessaging, Error>(directMessaging);
                else
                    return new Failure<DirectMessaging, Error>(DirectMessagingErrors.DirectMessagingNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<DirectMessaging, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will create a new direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the second participant</param>
        /// <returns>An either monad</returns>
        public Either<DirectMessaging, Error> Insert(long userId, long firstParticipantId, long secondParticipantId)
        {
            try
            {
                var isProperUser = firstParticipantId == userId
                                   || secondParticipantId == userId;

                var directMessaging = _burstChatContext
                    .DirectMessaging
                    .FirstOrDefault(dm => (dm.FirstParticipantUserId == firstParticipantId && dm.SecondParticipantUserId == secondParticipantId)
                                          || (dm.FirstParticipantUserId == secondParticipantId && dm.SecondParticipantUserId == firstParticipantId));

                if (isProperUser && directMessaging is null)
                {
                    var newDirectMessaging = new DirectMessaging
                    {
                        FirstParticipantUserId = firstParticipantId,
                        SecondParticipantUserId = secondParticipantId,
                    };

                    _burstChatContext
                        .DirectMessaging
                        .Add(newDirectMessaging);

                    _burstChatContext.SaveChanges();

                    return new Success<DirectMessaging, Error>(newDirectMessaging);
                }
                else
                    return new Failure<DirectMessaging, Error>(DirectMessagingErrors.DirectMessagingAlreadyExists());

            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<DirectMessaging, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the group</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Delete(long userId, long directMessagingId)
        {
            try
            {
                return Get(userId, directMessagingId).Bind(directMessaging =>
                {
                    _burstChatContext
                        .DirectMessaging
                        .Remove(directMessaging);

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
        ///   This method will fetch all available messages of a direct messaging entry.
        ///   If a target date is provided then 300 messages sent prior will be returned.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="targetDate">The date from which all prior messages will be fetched</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Message>, Error> GetMessages(long userId, long directMessagingId, DateTime? targetDate) =>
            GetWithMessages(userId, directMessagingId, targetDate)
                .Bind(directMessaging => new Success<IEnumerable<Message>, Error>(directMessaging.Messages));

        /// <summary>
        ///   This method will add a new message to a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the target direct messaging entry</param>
        /// <param name="message">The message instance that will be used for the insertion</param>
        /// <returns>An either monad</returns>
        public Either<Message, Error> InsertMessage(long userId, long directMessagingId, Message message)
        {
            try
            {
                return Get(userId, directMessagingId).Bind<Message>(directMessaging =>
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
                            DatePosted = message.DatePosted
                        };

                        directMessaging.Messages
                                    .Add(newMessage);

                        _burstChatContext.SaveChanges();

                        return new Success<Message, Error>(newMessage);
                    }
                    else
                        return new Failure<Message, Error>(DirectMessagingErrors.DirectMessagesNotFound());

                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will be edit a message of a direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message instance that will be used for the edit</param>
        /// <returns>An either monad<returns>
        public Either<Message, Error> UpdateMessage(long userId, long directMessagingId, Message message)
        {
            try
            {
                return GetWithMessages(userId, directMessagingId).Bind<Message>(directMessaging =>
                {
                    var messageEntry = directMessaging
                        .Messages
                        .FirstOrDefault(m => m.Id == message.Id);

                    if (messageEntry is { } && messageEntry.UserId == userId)
                    {
                        messageEntry.Links = message.GetLinksFromContent();
                        messageEntry.Content = message.RemoveLinksFromContent();
                        messageEntry.Edited = true;

                        _burstChatContext.SaveChanges();

                        return new Success<Message, Error>(messageEntry);
                    }
                    else
                        return new Failure<Message, Error>(DirectMessagingErrors.DirectMessagesNotFound());
                });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete a message from the direct messaging entry.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> DeleteMessage(long userId, long directMessagingId, long messageId)
        {
            try
            {
                return GetWithMessages(userId, directMessagingId).Bind<Unit>(directMessaging =>
                {
                    var messageEntry = directMessaging
                        .Messages
                        .FirstOrDefault(m => m.Id == messageId);

                    if (messageEntry is { } && messageEntry.UserId == userId)
                    {
                        directMessaging
                            .Messages
                            .Remove(messageEntry);

                        _burstChatContext.SaveChanges();

                        return new Success<Unit, Error>(new Unit());
                    }
                    else
                        return new Failure<Unit, Error>(DirectMessagingErrors.DirectMessagesNotFound());
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
