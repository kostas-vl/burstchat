using System;
using System.Collections.Generic;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Servers;

namespace BurstChat.Api.Services.ServersService
{
    /// <summary>
    ///   This interface exposes methods for fetching and manipulating server data.
    /// </summary>
    public interface IServersService
    {
        /// <summary>
        ///   This method will fetch information available for a server based on the provided
        ///   server id.
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An either monad</returns>
        Either<Server, Error> Get(int serverId);

        /// <summary>
        ///   This method will delete any information available for a server based on the provided
        ///   server id.
        /// </summary>
        /// <param name="serverId">The id of the server to be removed</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Delete(int serverId);

        /// <summary>
        ///   This method store information about a new server based on the provided Server instance.
        /// </summary>
        /// <param name="userId">The id of the user that creates the server</param>
        /// <param name="server">The server instance of which the information will be stored in the database</param>
        /// <returns>An either monad</returns>
        Either<Server, Error> Insert(long userId, Server server);

        /// <summary>
        ///   This method will update information about an existing server based on the provided server
        ///   instance.
        /// </summary>
        /// <param name="server">The server instance from which the information update will be based upon</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Update(Server server);

        /// <summary>
        ///     Fetches all invitations sent for a server based on the provided id.
        /// </summary>
        /// <param name="serverId">The id of the server</param>
        /// <returns>An either monad</returns>
        Either<IEnumerable<Invitation>, Error> GetInvitations(int serverId);

        /// <summary>
        ///     This method will create a new server invitation entry based on the provided parameters.
        /// </summary>
        /// <param name="serverId">The id of the server</param>
        /// <param name="userId">The id of the target user</param>
        /// <returns>An either monad</returns>
        Either<Invitation, Error> InsertInvitation(int serverId, long userId);
    }
}
