using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Servers;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Signal.Services.InvitationsService
{
    /// <summary>
    ///     This interface exposes methods for fetching and transforming server invitations sent to users.
    /// </summary>
    public interface IInvitationsService
    {
        /// <summary>
        ///     Fetches all invitations sent to an authenticated user.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<IEnumerable<Invitation>, Error>> GetAllAsync(HttpContext context);

        /// <summary>
        ///     Creates a new server invitation for a user based on the provided parameters.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="server">The id of the server the invitation is from</param>
        /// <param name="username">The name of the user the invitation will be sent</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<Invitation, Error>> InsertAsync(HttpContext context, int serverId, string username);

        /// <summary>
        ///     Updates a sent invitations based on the provided parameters.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="invitation">The invitations instance to be updated</param>
        /// <returns>A task of an either monad</returns>
        Task<Either<Invitation, Error>> UpdateAsync(HttpContext context, Invitation invitation);
    }
}
