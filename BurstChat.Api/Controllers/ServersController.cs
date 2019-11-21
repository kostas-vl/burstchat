using System;
using System.Collections.Generic;
using BurstChat.Api.Errors;
using BurstChat.Api.Services.ServersService;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Shared.Schema.Users;
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
        ///   Executes any necessary start up code for the controller.
        /// </summary>
        public ServersController(
            ILogger<ServersController> logger,
            IServersService serversService
        )
        {
            _logger = logger;
            _serversService = serversService;
        }

        /// <summary>
        ///   This method will fetch any available information about a server based on the provided id.
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{serverId:int}")]
        [ProducesResponseType(typeof(Server), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Get(int serverId) 
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _serversService.Get(userId, serverId));
            
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will create a new server based on the instance provided.
        /// </summary>
        /// <param name="server">The server instance to be created</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Server), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Post([FromBody] Server server)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _serversService.Insert(userId, server));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will update the properties of an existing server based on the provided instance.
        /// </summary>
        /// <param name="server">The server instance of which the properties will be used for the update</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Put([FromBody] Server server)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _serversService.Update(userId, server));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will remove an exising server based on the provided id.
        /// </summary>
        /// <param name="server">The id of the server to be removed</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{serverId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Delete(int serverId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId =>_serversService.Delete(userId, serverId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///     Fetches all users subscribed to the server of the id provided.
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{serverId:int}/subscribedUsers")]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetSubscribedUsers(int serverId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _serversService.GetSubscribedUsers(userId, serverId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///     Fetches all invitations sent out to users to join the server specified in the provided parameter.`
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{serverId:int}/invitations")]
        [ProducesResponseType(typeof(IEnumerable<Invitation>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetInvitations(int serverId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _serversService.GetInvitations(userId, serverId));
                
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///     Creates a new invitation to a user for the specified server.
        /// </summary>
        /// <param name="serverId">The id of the server</param>
        /// <param name="userId">The id of the target user</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("{serverId:int}/invitation")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult InsertInvitation(int serverId, [FromBody] string username)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _serversService.InsertInvitation(userId, serverId, username));

            return this.UnwrapMonad(monad);
        }
    }
}
