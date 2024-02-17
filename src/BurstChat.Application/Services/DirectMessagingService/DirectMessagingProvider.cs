using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Errors;
using BurstChat.Application.Extensions;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.DirectMessagingService;

public class DirectMessagingProvider : IDirectMessagingService
{
    private readonly ILogger<DirectMessagingProvider> _logger;
    private readonly IBurstChatContext _burstChatContext;

    public DirectMessagingProvider(
        ILogger<DirectMessagingProvider> logger,
        IBurstChatContext burstChatContext
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _burstChatContext = burstChatContext ?? throw new ArgumentNullException(nameof(burstChatContext));
    }


    public Result<DirectMessaging> Get(long userId, long directMessagingId) => _burstChatContext
        .Map(bc => bc
            .DirectMessaging
            .FirstOrDefault(dm =>
                dm.Id == directMessagingId
                && (dm.FirstParticipantUserId == userId
                    || dm.SecondParticipantUserId == userId))
        )
        .And(dm => dm?.Ok() ?? DirectMessagingErrors.DirectMessageNotFound)
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<DirectMessaging> Get(long userId, long firstParticipantId, long secondParticipantId) =>
        (
            userId != firstParticipantId && userId != secondParticipantId
                ? DirectMessagingErrors.DirectMessageNotFound
                : Unit.Ok
        )
        .Map(_ => _burstChatContext
            .DirectMessaging
            .Include(dm => dm.FirstParticipantUser)
            .Include(dm => dm.SecondParticipantUser)
            .FirstOrDefault(dm =>
                (dm.FirstParticipantUserId == firstParticipantId && dm.SecondParticipantUserId == secondParticipantId)
                || (dm.FirstParticipantUserId == secondParticipantId && dm.SecondParticipantUserId == firstParticipantId))
        )
        .And(dm => dm?.Ok() ?? DirectMessagingErrors.DirectMessageNotFound)
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<IEnumerable<User?>> GetUsers(long userId) => _burstChatContext
        .Map(bc => bc
            .DirectMessaging
            .Include(dm => dm.FirstParticipantUser)
            .Include(dm => dm.SecondParticipantUser)
            .Where(dm => dm.FirstParticipantUserId == userId
                         || dm.SecondParticipantUserId == userId)
            .ToList()
            .Select(dm => dm.FirstParticipantUserId != userId
                          ? dm.FirstParticipantUser
                          : dm.SecondParticipantUser)
            .ToList()
            .AsEnumerable()
        )
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<DirectMessaging> Insert(long userId, long firstParticipantId, long secondParticipantId) => _burstChatContext
        .And(bc =>
        {
            var isProperUser = firstParticipantId == userId
                               || secondParticipantId == userId;

            var directMessaging = bc
                .DirectMessaging
                .FirstOrDefault(dm => (dm.FirstParticipantUserId == firstParticipantId && dm.SecondParticipantUserId == secondParticipantId)
                                      || (dm.FirstParticipantUserId == secondParticipantId && dm.SecondParticipantUserId == firstParticipantId));

            if (!isProperUser || directMessaging is not null)
                return DirectMessagingErrors.DirectMessagingAlreadyExists;

            var newDirectMessaging = new DirectMessaging
            {
                FirstParticipantUserId = firstParticipantId,
                SecondParticipantUserId = secondParticipantId,
            };
            bc.DirectMessaging.Add(newDirectMessaging);
            bc.SaveChanges();

            return newDirectMessaging.Ok();
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<DirectMessaging> Delete(long userId, long directMessagingId) => Get(userId, directMessagingId)
        .Map(dm=>
        {
            _burstChatContext.DirectMessaging.Remove(dm);
            return dm;
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<IEnumerable<Message>> GetMessages(
        long userId,
        long directMessagingId,
        string? searchTerm = null,
        long? lastMessageId = null
    ) => Get(userId, directMessagingId)
        .Map(_ => _burstChatContext
            .DirectMessaging
            .Include(dm => dm.Messages)
            .ThenInclude(m => m.User)
            .Include(dm => dm.Messages)
            .ThenInclude(m => m.Links)
            .Where(dm => dm.Id == directMessagingId)
            .Select(dm => dm.Messages
                            .Where(m => m.Id < (lastMessageId ?? long.MaxValue)
                                        && (searchTerm == null || m.Content.Contains(searchTerm)))
                            .OrderByDescending(m => m.Id)
                            .Take(100))
            .ToList()
            .Aggregate(new List<Message>(), (current, next) =>
            {
                current.AddRange(next);
                return current;
            })
            .OrderBy(m => m.Id)
            .ToList()
            .AsEnumerable()
        )
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Message> InsertMessage(long userId, long directMessagingId, Message message) => Get(userId, directMessagingId)
        .And(dm =>
        {
            var user = _burstChatContext
                .Users
                .FirstOrDefault(u => u.Id == userId);

            if (user is null || message is null)
                return DirectMessagingErrors.DirectMessagesNotFound;

            var newMessage = new Message
            {
                User = user,
                Links = message.GetLinksFromContent(),
                Content = message.RemoveLinksFromContent(),
                Edited = false,
                DatePosted = message.DatePosted
            };
            dm.Messages.Add(newMessage);
            _burstChatContext.SaveChanges();

            return newMessage.Ok();
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Message> UpdateMessage(long userId, long directMessagingId, Message message) =>
        (message?.Ok() ?? DirectMessagingErrors.DirectMessageNotFound)
        .And(_ => Get(userId, directMessagingId))
        .And(_ =>
        {
            var entries = _burstChatContext
                .DirectMessaging
                .Include(dm => dm.Messages)
                .ThenInclude(dm => dm.User)
                .Include(dm => dm.Messages)
                .ThenInclude(dm => dm.Links)
                .Where(dm => dm.Id == directMessagingId)
                .Select(dm => dm.Messages.FirstOrDefault(m => m.Id == message!.Id))
                .ToList();

            if (entries.Count != 1)
                return DirectMessagingErrors.DirectMessageNotFound;

            var entry = entries.First()!;
            entry.Links = message!.GetLinksFromContent();
            entry.Content = message!.RemoveLinksFromContent();
            entry.Edited = true;
            _burstChatContext.SaveChanges();

            return entry.Ok();
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Message> DeleteMessage(long userId, long directMessagingId, long messageId) => Get(userId, directMessagingId)
        .And(_ =>
        {
            var directMessaging = _burstChatContext
                .DirectMessaging
                .Include(dm => dm.Messages.Where(m => m.Id == messageId))
                .First(dm => dm.Id == directMessagingId);

            if (!directMessaging.Messages.Any())
                return DirectMessagingErrors.DirectMessagesNotFound;

            var message = directMessaging.Messages.First()!;
            directMessaging.Messages.Remove(message);
            _burstChatContext.SaveChanges();

            return message.Ok();
        })
        .InspectErr(e => _logger.LogError(e.Message));
}
