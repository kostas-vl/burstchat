using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Signal.Services.ApiInteropService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal.Services.InvitationsService
{
    /// <summary>
    ///     This class is the based implementation of the IInvitationsService.
    /// </summary>
    public class InvitationsProvider : IInvitationsService
    {
        private readonly ILogger<InvitationsProvider> _logger;
        private readonly BurstChatApiInteropService _apiInteropService;

        /// <summary>
        ///     Creates a new instance of InvitationsProvider.
        /// </summary>
        public InvitationsProvider(
            ILogger<InvitationsProvider> logger,
            BurstChatApiInteropService apiInteropService
        )
        {
            _logger = logger;
            _apiInteropService = apiInteropService;
        }

        /// <summary>
        ///     Fetches all invitations sent to an authenticated user.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>An either monad</returns>
        public async Task<Either<IEnumerable<Invitation>, Error>> GetAllAsync(HttpContext context)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = $"api/user/invitations";

                return await _apiInteropService.SendAsync<IEnumerable<Invitation>>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<Invitation>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///     Creates a new server invitation for a user based on the provided parameters.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="server">The id of the server the invitation is from</param>
        /// <param name="username">The name of the user the invitation will be sent</param>
        /// <returns>A task of an either monad</returns>
        public async Task<Either<Invitation, Error>> InsertAsync(HttpContext context, int serverId, string username)
        {
            try
            {
                var method = HttpMethod.Post;
                var url = $"api/servers/{serverId}/invitation";
                var content = new StringContent($"\"{username}\"", Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<Invitation>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Invitation, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///     Updates a sent invitations based on the provided parameters.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="invitation">The invitations instance to be updated</param>
        /// <returns>A task of an either monad</returns>
        public async Task<Either<Invitation, Error>> UpdateAsync(HttpContext context, Invitation invitation)
        {
            try
            {
                var method = HttpMethod.Put;
                var url = $"/api/user/invitation";
                var jsonMessage = JsonSerializer.Serialize(invitation);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<Invitation>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Invitation, Error>(SystemErrors.Exception());
            }
        }
    }
}
