using System.Collections.Generic;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;

namespace BurstChat.Application.Services.ServersService
{
    /// <summary>
    ///   This interface exposes methods for fetching and manipulating server data.
    /// </summary>
    public interface IServersService
    {
        /// <summary>
        /// This method will fetch information available for a server based on the provided
        /// server id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An either monad</returns>
        Result<Server> Get(long userId, int serverId);

        /// <summary>
        /// This method will delete any information available for a server based on the provided
        /// server id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server to be removed</param>
        /// <returns>An either monad</returns>
        Result<Unit> Delete(long userId, int serverId);

        /// <summary>
        /// This method store information about a new server based on the provided Server instance.
        /// </summary>
        /// <param name="userId">The id of the user that creates the server</param>
        /// <param name="server">The server instance of which the information will be stored in the database</param>
        /// <returns>An either monad</returns>
        Result<Server> Insert(long userId, Server server);

        /// <summary>
        /// This method will update information about an existing server based on the provided server
        /// instance.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="server">The server instance from which the information update will be based upon</param>
        /// <returns>An either monad</returns>
        Result<Server> Update(long userId, Server server);

        /// <summary>
        /// Fetches all users subscribed to the server based on the server id provided.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An either monad</returns>
        Result<IEnumerable<User>> GetSubscribedUsers(long userId, int serverId);

        /// <summary>
        /// Removes a subscription from an existing server.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server</param>
        /// <param name="subscription">The subscription instance to be removed</param>
        /// <returns>An either monad</returns>
        Result<Subscription> DeleteSubscription(
            long userId,
            int serverId,
            Subscription subscription
        );

        /// <summary>
        /// Fetches all invitations sent for a server based on the provided id.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server</param>
        /// <returns>An either monad</returns>
        Result<IEnumerable<Invitation>> GetInvitations(long userId, int serverId);

        /// <summary>
        /// This method will create a new server invitation entry based on the provided parameters.
        /// </summary>
        /// <param name="userId">The id of the requesting user</param>
        /// <param name="serverId">The id of the server</param>
        /// <param name="username">The name of the target user</param>
        /// <returns>An either monad</returns>
        Result<Invitation> InsertInvitation(long userId, int serverId, string username);
    }
}
