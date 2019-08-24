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
                    .Include(s => s.SubscribedUsers)
                    .FirstOrDefault(s => s.Id == serverId);

                if (server != null)
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
                return Get(serverId)
                    .Bind(server => 
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
        /// <param name="server">The server instance of which the information will be stored in the database</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Insert(Server server)
        {
            try
            {
                var serverEntry = new Server
                {
                    Name = server.Name,
                    DateCreated = server.DateCreated
                };

                _burstChatContext
                    .Servers
                    .Add(serverEntry);

                _burstChatContext.SaveChanges();

                return new Success<Unit, Error>(new Unit());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
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
                return Get(server.Id)
                    .Bind(s =>
                    {
                        s.Name = server.Name;

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
    }
}
