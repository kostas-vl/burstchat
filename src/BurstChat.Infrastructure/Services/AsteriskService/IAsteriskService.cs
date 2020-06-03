using System;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Models;

namespace BurstChat.Infrastructure.Services.AsteriskService
{
    /// <summary>
    /// This interface represents a contract that all services that need to interact with an Asterisk server.
    /// </summary>
    public interface IAsteriskService
    {
        /// <summary>
        /// Sends an Http request to a remore Asterisk server and returns information
        /// about a pjsip endpoint, if that exists.
        /// </summary>
        /// <param name="endpoint">The endpoint name</param>
        /// <returns>An either monad</returns>
        Task<Either<AsteriskEndpoint, Error>> GetAsync(Guid endpoint);

        /// <summary>
        /// Sends an Http request to a remote Asterisk server in order to create a new
        /// pjsip aor, auth and endpoint. If the operations are all successful an instance
        /// of AsteriskEndpoint is returned.
        /// </summary>
        /// <param name="endpoint">The endpoint name</param>
        /// <param name="password">The password for the endpoint</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<AsteriskEndpoint, Error>> PostAsync(Guid endpoint, Guid password);
    }
}
