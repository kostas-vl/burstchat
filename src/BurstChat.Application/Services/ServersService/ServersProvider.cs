using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Application.Errors;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.ServersService
{
    /// <summary>
    ///   This class represents the base implementation of the IServersService.
    /// </summary>
    public class ServersProvider : IServersService
    {
        private readonly ILogger<ServersProvider> _logger;
        private readonly IBurstChatContext _burstChatContext;

        /// <summary>
        ///   Executes any necessary start up code for the service.
        /// </summary>
        public ServersProvider(
            ILogger<ServersProvider> logger,
            IBurstChatContext burstChatContext
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
        }

        /// <summary>
        ///   This method will fetch information available for a server based on the provided
        ///   server id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An either monad</returns>
        public Either<Server, Error> Get(long userId, int serverId)
        {
            try
            {
                var server = _burstChatContext
                    .Servers
                    .Include(s => s.Channels)
                    .Include(s => s.Subscriptions)
                    .FirstOrDefault(s => s.Id == serverId);

                var serverExists = server is { }
                                   && server.Subscriptions.Any(s => s.UserId == userId);

                if (serverExists)
                    return new Success<Server, Error>(server!);
                else
                    return new Failure<Server, Error>(ServerErrors.ServerNotFound());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Server, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete any information available for a server based on the provided
        ///   server id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server to be removed</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Delete(long userId, int serverId)
        {
            try
            {
                return Get(userId, serverId).Bind(server =>
                {
                    _burstChatContext
                        .Servers
                        .Remove(server);

                    _burstChatContext.SaveChanges();

                    return new Success<Unit, Error>(new Unit());
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method store information about a new server based on the provided Server instance.
        /// </summary>
        /// <param name="userId">The id of the user that creates the server</param>
        /// <param name="server">The server instance of which the information will be stored in the database</param>
        /// <returns>An either monad</returns>
        public Either<Server, Error> Insert(long userId, Server server)
        {
            try
            {
                var existingServer = _burstChatContext
                    .Servers
                    .FirstOrDefault(s => s.Name == server.Name);

                if (existingServer is null)
                {
                    var serverEntry = new Server
                    {
                        Name = server.Name,
                        DateCreated = server.DateCreated,
                        Subscriptions = new List<Subscription>
                        {
                            new Subscription
                            {
                                UserId = userId
                            }
                        }
                    };

                    _burstChatContext
                        .Servers
                        .Add(serverEntry);

                    _burstChatContext.SaveChanges();

                    return new Success<Server, Error>(serverEntry);
                }
                else
                    return new Failure<Server, Error>(ServerErrors.ServerAlreadyExists());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Server, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will update information about an existing server based on the provided server
        ///   instance.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="server">The server instance from which the information update will be based upon</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Update(long userId, Server server)
        {
            try
            {
                return Get(userId, server.Id).Bind(serverEntry =>
                {
                    serverEntry.Name = server.Name;
                    serverEntry.Avatar = server.Avatar;

                    _burstChatContext.SaveChanges();

                    return new Success<Unit, Error>(new Unit());
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///     Fetches all users subscribed to the server based on the server id provided.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<User>, Error> GetSubscribedUsers(long userId, int serverId)
        {
            try
            {
                return Get(userId, serverId).Bind(server =>
                {
                    var users = _burstChatContext
                        .Users
                        .AsEnumerable()
                        .Where(u => server.Subscriptions
                                          .Any(s => s.UserId == u.Id))
                        .ToList();

                    return new Success<IEnumerable<User>, Error>(users);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<User>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Removes a user from an existing server.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server</param>
        /// <param name="subscription">The subscription instance to be removed</param>
        /// <returns>An either monad</returns>
        public Either<Subscription, Error> DeleteSubscription(long userId, int serverId, Subscription subscription)
        {
            try
            {
                return Get(userId, serverId).Bind<Subscription>(server =>
                {
                    var targetSubscription = server
                        .Subscriptions
                        .FirstOrDefault(s => s.Id == subscription.Id);

                    if (targetSubscription is null)
                        return new Failure<Subscription, Error>(UserErrors.UserNotFound());

                    server
                        .Subscriptions
                        .Remove(targetSubscription);

                    _burstChatContext.SaveChanges();

                    return new Success<Subscription, Error>(targetSubscription);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Subscription, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///     Fetches all invitations sent for a server based on the provided id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Invitation>, Error> GetInvitations(long userId, int serverId)
        {
            try
            {
                return Get(userId, serverId).Bind(server =>
                {
                    var invitations = _burstChatContext
                        .Invitations
                        .Where(i => i.ServerId == serverId)
                        .ToList();

                    return new Success<IEnumerable<Invitation>, Error>(invitations);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<Invitation>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///     This method will create a new server invitation entry based on the provided parameters.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server</param>
        /// <param name="username">The name of the target user</param>
        /// <returns>An either monad</returns>
        public Either<Invitation, Error> InsertInvitation(long userId, int serverId, string username)
        {
            try
            {
                return Get(userId, serverId).Bind<Invitation>(server =>
                {
                    var userExists = server
                        .Subscriptions
                        .Any(s => s.UserId == userId);

                    var targetUser = _burstChatContext
                        .Users
                        .FirstOrDefault(u => u.Name == username);

                    var targetAlreadyMember = server
                        .Subscriptions
                        .Any(u => targetUser is { }
                                  && u.UserId == targetUser.Id);

                    if (!userExists)
                        return new Failure<Invitation, Error>(ServerErrors.UserAlreadyMember());

                    if (targetAlreadyMember)
                        return new Failure<Invitation, Error>(ServerErrors.UserAlreadyMember());

                    var invitation = new Invitation
                    {
                        ServerId = serverId,
                        UserId = targetUser.Id,
                        Accepted = false,
                        Declined = false,
                        DateUpdated = null,
                        DateCreated = DateTime.Now
                    };

                    _burstChatContext
                        .Invitations
                        .Add(invitation);

                    _burstChatContext.SaveChanges();

                    return new Success<Invitation, Error>(invitation);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Invitation, Error>(SystemErrors.Exception());
            }
        }
    }
}
