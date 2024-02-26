using System;
using System.Collections.Generic;
using System.Linq;
using BurstChat.Application.Errors;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.ServersService;

public class ServersProvider : IServersService
{
    private readonly ILogger<ServersProvider> _logger;
    private readonly IBurstChatContext _burstChatContext;

    public ServersProvider(ILogger<ServersProvider> logger, IBurstChatContext burstChatContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _burstChatContext =
            burstChatContext ?? throw new ArgumentNullException(nameof(burstChatContext));
    }

    public Result<Server> Get(long userId, int serverId) =>
        _burstChatContext
            .Map(bc =>
                bc.Servers.Include(s => s.Channels)
                    .Include(s => s.Subscriptions)
                    .FirstOrDefault(s => s.Id == serverId)
            )
            .And(srv =>
                srv is not null && srv.Subscriptions.Any(s => s.UserId == userId)
                    ? srv.Ok()
                    : ServerErrors.ServerNotFound
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Unit> Delete(long userId, int serverId) =>
        Get(userId, serverId)
            .Map(server =>
            {
                _burstChatContext.Servers.Remove(server);
                _burstChatContext.SaveChanges();
                return new Unit();
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Server> Insert(long userId, Server server) =>
        _burstChatContext
            .And(bc =>
            {
                return bc.Servers.FirstOrDefault(s => s.Name == server.Name) is null
                    ? Unit.Ok
                    : ServerErrors.ServerAlreadyExists;
            })
            .And(_ =>
            {
                var serverEntry = new Server
                {
                    Name = server.Name,
                    DateCreated = server.DateCreated,
                    Subscriptions = new List<Subscription> { new() { UserId = userId } }
                };

                _burstChatContext.Servers.Add(serverEntry);
                _burstChatContext.SaveChanges();
                return serverEntry.Ok();
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Server> Update(long userId, Server server) =>
        Get(userId, server.Id)
            .Map(serverEntry =>
            {
                serverEntry.Name = server.Name;
                serverEntry.Avatar = server.Avatar;
                _burstChatContext.SaveChanges();
                return serverEntry;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<IEnumerable<User>> GetSubscribedUsers(long userId, int serverId) =>
        Get(userId, serverId)
            .Map(server =>
                _burstChatContext
                    .Users.AsEnumerable()
                    .Where(u => server.Subscriptions.Any(s => s.UserId == u.Id))
                    .ToList()
                    .AsEnumerable()
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Subscription> DeleteSubscription(
        long userId,
        int serverId,
        Subscription subscription
    ) =>
        Get(userId, serverId)
            .And(server =>
            {
                var targetSubscription = server.Subscriptions.FirstOrDefault(s =>
                    s.Id == subscription.Id
                );

                if (targetSubscription is null)
                    return UserErrors.UserNotFound;

                server.Subscriptions.Remove(targetSubscription);

                _burstChatContext.SaveChanges();

                return targetSubscription.Ok();
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<IEnumerable<Invitation>> GetInvitations(long userId, int serverId) =>
        Get(userId, serverId)
            .Map(server =>
                _burstChatContext
                    .Invitations.Where(i => i.ServerId == serverId)
                    .ToList()
                    .AsEnumerable()
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Invitation> InsertInvitation(long userId, int serverId, string username) =>
        Get(userId, serverId)
            .And(server =>
            {
                var userExists = server.Subscriptions.Any(s => s.UserId == userId);

                var targetUser = _burstChatContext.Users.First(u => u.Name == username);

                var targetAlreadyMember = server.Subscriptions.Any(u =>
                    targetUser is { } && u.UserId == targetUser.Id
                );

                if (!userExists)
                    return ServerErrors.UserAlreadyMember;

                if (targetAlreadyMember)
                    return ServerErrors.UserAlreadyMember;

                var invitation = new Invitation
                {
                    ServerId = serverId,
                    UserId = targetUser.Id,
                    Accepted = false,
                    Declined = false,
                    DateUpdated = null,
                    DateCreated = DateTime.UtcNow
                };

                _burstChatContext.Invitations.Add(invitation);
                _burstChatContext.SaveChanges();
                return invitation.Ok();
            })
            .InspectErr(e => _logger.LogError(e.Message));
}
