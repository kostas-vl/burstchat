using System;
using System.Collections.Generic;
using BurstChat.Api.ActionResults;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.ServersService;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    /// This controllers exposes endpoints for listing, subscribing and leaving servers.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/servers")]
    [Produces("application/json")]
    public class ServersController : ControllerBase
    {
        private readonly ILogger<ServersController> _logger;
        private readonly IServersService _serversService;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// 
        /// Exceptions:
        ///     ArgumentNullException: When any parameter is null.
        /// </summary>
        public ServersController(
            ILogger<ServersController> logger,
            IServersService serversService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serversService = serversService ?? throw new ArgumentNullException(nameof(serversService));
        }

        /// <summary>
        /// This method will fetch any available information about a server based on the provided id.
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("{serverId:int}")]
        [ProducesResponseType(typeof(Server), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Server, Error> Get(int serverId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _serversService.Get(userId, serverId));

        /// <summary>
        /// This method will create a new server based on the instance provided.
        /// </summary>
        /// <param name="server">The server instance to be created</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Server), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Server, Error> Post([FromBody] Server server) =>
            HttpContext.GetUserId()
                       .Bind(userId => _serversService.Insert(userId, server));

        /// <summary>
        /// This method will update the properties of an existing server based on the provided instance.
        /// </summary>
        /// <param name="server">The server instance of which the properties will be used for the update</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Server, Error> Put([FromBody] Server server) =>
            HttpContext.GetUserId()
                       .Bind(userId => _serversService.Update(userId, server));

        /// <summary>
        /// This method will remove an exising server based on the provided id.
        /// </summary>
        /// <param name="server">The id of the server to be removed</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpDelete("{serverId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Unit, Error> Delete(int serverId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _serversService.Delete(userId, serverId));

        /// <summary>
        /// Fetches all users subscribed to the server of the id provided.
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("{serverId:int}/subscribedUsers")]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<User>, Error> GetSubscribedUsers(int serverId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _serversService.GetSubscribedUsers(userId, serverId));

        /// <summary>
        /// Removes a subscription from an existing server.
        /// </summary>
        /// <param name="serverId">The id of the server</param>
        /// <param name="subscription">The instance of the subscription to be removed</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpDelete("{serverId:int}/subscriptions")]
        [ProducesResponseType(typeof(Subscription), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Subscription, Error> DeleteSubscription(int serverId, [FromBody] Subscription subscription) =>
            HttpContext.GetUserId()
                       .Bind(userId => _serversService.DeleteSubscription(userId, serverId, subscription));

        /// <summary>
        /// Fetches all invitations sent out to users to join the server specified in the provided parameter.`
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("{serverId:int}/invitations")]
        [ProducesResponseType(typeof(IEnumerable<Invitation>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<Invitation>, Error> GetInvitations(int serverId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _serversService.GetInvitations(userId, serverId));

        /// <summary>
        /// Creates a new invitation to a user for the specified server.
        /// </summary>
        /// <param name="serverId">The id of the server</param>
        /// <param name="userId">The id of the target user</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost("{serverId:int}/invitation")]
        [ProducesResponseType(typeof(Invitation), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Invitation, Error> InsertInvitation(int serverId, [FromBody] string username) =>
            HttpContext.GetUserId()
                       .Bind(userId => _serversService.InsertInvitation(userId, serverId, username));
    }
}
