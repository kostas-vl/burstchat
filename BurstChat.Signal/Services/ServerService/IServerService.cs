using System;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Servers;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Signal.Services.ServerService
{
    /// <summary>
    ///     This interface exposes methods for fetching and transforming server data.
    /// </summary>
    public interface IServerService
    {
        /// <summary>
        ///     Fetches information about a server based on the provided parameters.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<Server, Error>> GetAsync(HttpContext context, int serverId);

        /// <summary>
        ///     Requests the createion of a new server based on the provided id.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="server">The server instance to be created</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<Server, Error>> PostAsync(HttpContext context, Server server);
    }
}