using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Servers;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Signal.Services.ServerService
{
    /// <summary>
    /// This interface exposes methods for fetching and transforming server data.
    /// </summary>
    public interface IServerService
    {
        /// <summary>
        /// Fetches information about a server based on the provided parameters.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<Server, Error>> GetAsync(HttpContext context, int serverId);

        /// <summary>
        /// Requests the creation of a new server based on the provided id.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="server">The server instance to be created</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<Server, Error>> PostAsync(HttpContext context, Server server);

        /// <summary>
        /// Removes a subscription from an existing server.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="serverId">The id of the server</param>
        /// <param name="subscription">The subscription to be removed</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<Subscription, Error>> DeleteSubscription(HttpContext context, int serverId, Subscription subscription);
    }
}
