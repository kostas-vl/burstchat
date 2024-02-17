using System;
using System.Collections.Generic;
using System.Linq;
using BurstChat.Application.Errors;
using BurstChat.Application.Extensions;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.UserService;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.PrivateGroupsService;

public class PrivateGroupsProvider : IPrivateGroupsService
{
    private readonly ILogger<PrivateGroupsProvider> _logger;
    private readonly IBurstChatContext _burstChatContext;
    private readonly IUserService _userService;

    public PrivateGroupsProvider(
        ILogger<PrivateGroupsProvider> logger,
        IBurstChatContext burstChatContext,
        IUserService userService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _burstChatContext =
            burstChatContext ?? throw new ArgumentNullException(nameof(burstChatContext));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public Result<PrivateGroup> Get(long userId, long groupId) =>
        _burstChatContext
            .And(bc =>
            {
                var privateGroup = bc
                    .PrivateGroups.Include(pgm => pgm.Users)
                    .FirstOrDefault(pgm => pgm.Id == groupId);

                var userInList = privateGroup?.Users.Any(u => u.Id == userId) ?? false;

                return privateGroup is { } && userInList
                    ? privateGroup.Ok()
                    : PrivateGroupErrors.GroupNotFound;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<PrivateGroup> Insert(long userId, string groupName) =>
        _burstChatContext
            .And(bc =>
            {
                var privateGroup = _burstChatContext.PrivateGroups.FirstOrDefault(pgm =>
                    pgm.Name == groupName
                );

                if (privateGroup is null)
                    return Unit.Ok;
                else
                    return PrivateGroupErrors.GroupNotFound;
            })
            .Map(_ =>
            {
                var newPrivateGroup = new PrivateGroup
                {
                    Name = groupName,
                    DateCreated = DateTime.UtcNow,
                };

                var user = _burstChatContext.Users.FirstOrDefault(u => u.Id == userId);

                if (user is not null)
                    newPrivateGroup.Users.Add(user);

                _burstChatContext.PrivateGroups.Add(newPrivateGroup);
                _burstChatContext.SaveChanges();
                return newPrivateGroup;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Unit> Delete(long userId, long groupId) =>
        Get(userId, groupId)
            .Map(privateGroup =>
            {
                _burstChatContext.PrivateGroups.Remove(privateGroup);
                _burstChatContext.SaveChanges();
                return Unit.Instance;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<PrivateGroup> InsertUser(long userId, long groupId, long newUserId) =>
        Get(userId, groupId)
            .And(privateGroup =>
                _burstChatContext.Users.FirstOrDefault(u => u.Id == newUserId) is User user
                    ? (privateGroup, user).Ok()
                    : UserErrors.UserNotFound
            )
            .Map(data =>
            {
                var (privateGroup, user) = data;
                privateGroup.Users.Add(user);
                _burstChatContext.SaveChanges();
                return privateGroup;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<PrivateGroup> InsertUsers(long userId, long groupId, IEnumerable<long> userIds) =>
        Get(userId, groupId)
            .Map(privateGroup =>
            {
                var idsToBeAdded = privateGroup.Users.Where(u => !userIds.Contains(u.Id));
                var users = _burstChatContext
                    .Users.AsEnumerable()
                    .Where(u => userIds.Contains(u.Id))
                    .ToList();

                privateGroup.Users.AddRange(users);
                _burstChatContext.SaveChanges();
                return privateGroup;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<PrivateGroup> DeleteUser(long userId, long groupId, long targetUserId) =>
        Get(userId, groupId)
            .And(privateGroup =>
                _burstChatContext.Users.FirstOrDefault(u => u.Id == targetUserId) is User user
                    ? (privateGroup, user).Ok()
                    : UserErrors.UserNotFound
            )
            .Map(data =>
            {
                var (privateGroup, user) = data;
                privateGroup.Users.Remove(user);
                _burstChatContext.SaveChanges();
                return privateGroup;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<IEnumerable<Message>> GetMessages(long userId, long groupId) =>
        _burstChatContext
            .Map(bc =>
                bc.PrivateGroups.Include(pgm => pgm.Messages)
                    .FirstOrDefault(pgm => pgm.Id == groupId)
            )
            .And(pg => pg?.Ok() ?? PrivateGroupErrors.GroupNotFound)
            .And(pg =>
            {
                var userInList = pg.Users.Any(u => u.Id == userId);
                return userInList
                    ? pg.Messages.AsEnumerable().Ok()
                    : PrivateGroupErrors.GroupNotFound;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Message> InsertMessage(long userId, long groupId, Message message) =>
        Get(userId, groupId)
            .And(grp =>
            {
                var user = _burstChatContext.Users.FirstOrDefault(u => u.Id == userId);

                if (user is null || message is null)
                    return PrivateGroupErrors.GroupNotFound;

                var newMessage = new Message
                {
                    User = user,
                    Content = message.Content,
                    Edited = false,
                    DatePosted = DateTime.UtcNow
                };
                grp.Messages.Add(newMessage);
                _burstChatContext.SaveChanges();
                return newMessage.Ok();
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Message> UpdateMessage(long userId, long groupId, Message message) =>
        GetMessages(userId, groupId)
            .Map(gm => gm.FirstOrDefault(m => m.Id == message.Id))
            .And(sm =>
                sm is not null && message is not null
                    ? sm.Ok()
                    : PrivateGroupErrors.GroupMessageNotFound
            )
            .Map(sm =>
            {
                sm.Links = message.GetLinksFromContent();
                sm.Content = message.RemoveLinksFromContent();
                sm.Edited = true;
                _burstChatContext.SaveChanges();
                return sm;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Unit> DeleteMessage(long userId, long groupId, long messageId) =>
        GetMessages(userId, groupId)
            .Map(gm => gm.FirstOrDefault(m => m.Id == messageId))
            .And(sm => sm?.Ok() ?? PrivateGroupErrors.GroupMessageNotFound)
            .Map(sm =>
            {
                _burstChatContext.Messages.Remove(sm);
                _burstChatContext.SaveChanges();
                return Unit.Instance;
            })
            .InspectErr(e => _logger.LogError(e.Message));
}
