using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Api.Errors;
using BurstChat.Shared.Context;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Servers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Services.ServersService
{
    /// <summary>
    ///   This class represents the base implementation of the IServersService.
    /// </summary>
    public class ServersProvider : IServersService
    {
        private readonly ILogger<ServersProvider> _logger;
        private readonly BurstChatContext _burstChatContext;

        /// <summary>
        ///   Executes any necessary start up code for the service.
        /// </summary>
        public ServersProvider(
            ILogger<ServersProvider> logger,
            BurstChatContext burstChatContext
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
        }

        /// <summary>
        ///   This method will fetch information available for a server based on the provided
        ///   server id.
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An either monad</returns>
        public Either<Server, Error> Get(int serverId)
        {
            try
            {
                var server = _burstChatContext
                    .Servers
                    .Include(s => s.Channels)
                    .Include(s => s.Subscriptions)
                    .FirstOrDefault(s => s.Id == serverId);

                if (server is { })
                    return new Success<Server, Error>(server);
                else
                    return new Failure<Server, Error>(ServerErrors.ServerNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Server, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete any information available for a server based on the provided
        ///   server id.
        /// </summary>
        /// <param name="serverId">The id of the server to be removed</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Delete(int serverId)
        {
            try
            {
                return Get(serverId).Bind(server =>
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
                _logger.LogException(e);
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
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Server, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will update information about an existing server based on the provided server
        ///   instance.
        /// </summary>
        /// <param name="server">The server instance from which the information update will be based upon</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Update(Server server)
        {
            try
            {
                return Get(server.Id).Bind(serverEntry =>
                {
                    serverEntry.Name = server.Name;

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
        ///     Fetches all invitations sent for a server based on the provided id.
        /// </summary>
        /// <param name="serverId">The id of the server</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Invitation>, Error> GetInvitations(int serverId)
        {
            try
            {
                var invitations = _burstChatContext
                    .Invitations
                    .Where(i => i.ServerId == serverId)
                    .ToList();

                return new Success<IEnumerable<Invitation>, Error>(invitations);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<IEnumerable<Invitation>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///     This method will create a new server invitation entry based on the provided parameters.
        /// </summary>
        /// <param name="serverId">The id of the server</param>
        /// <param name="userId">The id of the target user</param>
        /// <returns>An either monad</returns>
        public Either<Invitation, Error> InsertInvitation(int serverId, long userId)
        {
            try
            {
                return Get(serverId).Bind<Invitation>(server =>
                {
                    var userExists = server
                        .Subscriptions
                        .Any(s => s.UserId == userId);

                    if (userExists)
                        return new Failure<Invitation, Error>(ServerErrors.UserAlreadyMember());

                    var invitation = new Invitation
                    {
                        ServerId = serverId,
                        UserId = userId,
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
                _logger.LogException(e);
                return new Failure<Invitation, Error>(SystemErrors.Exception());
            }
        }
    }
}
