using System.Collections.Generic;
using BurstChat.Api.ActionResults;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.UserService;
using BurstChat.Domain.Schema.Users;
using BurstChat.Domain.Schema.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    /// This class represents an ASPNET Core controller that exposes endpoints for interacting with
    /// user data.
    /// </summary>
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// </summary>
        public UserController(
            ILogger<UserController> logger,
            IUserService userService
        )
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        ///   This method fetches information about an authenticated user.
        /// </summary>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<User, Error> Get() =>
            HttpContext.GetUserId()
                       .Bind(_userService.Get);

        /// <summary>
        ///   This method will update the properties of a user based on the provided instance.
        /// </summary>
        /// <param name="user">The user instance to be user in the update</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Unit, Error> Put([FromBody] User user) =>
            _userService.Update(user);

        /// <summary>
        ///   This method will delete an existing user based on the provided id.
        /// </summary>
        /// <param name="id">The id of the user to be delete</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Unit, Error> Delete(long id) =>
            HttpContext.GetUserId()
                       .Bind(_userService.Delete);

        /// <summary>
        ///   This method will fetch all subscribed servers of a user based on the provided user id.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("subscriptions")]
        [ProducesResponseType(typeof(IEnumerable<Server>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<Server>, Error> GetSubscriptions() =>
            HttpContext.GetUserId()
                       .Bind(_userService.GetSubscriptions);

        /// <summary>
        ///   This method will fetch all private groups that the user with the provided id is part of.
        /// </summary>
        /// <returns>An MonadActionResult instance</returns>
        [HttpGet("groups")]
        [ProducesResponseType(typeof(IEnumerable<PrivateGroup>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<PrivateGroup>, Error> GetPrivateGroups() =>
            HttpContext.GetUserId()
                       .Bind(_userService.GetPrivateGroups);

        /// <summary>
        ///     This method will fetch all direct messaging entries that the user with the provided id
        ///     is part of.
        /// </summary>
        /// <returns>An MonadActionResult instance</returns>
        [HttpGet("direct")]
        [ProducesResponseType(typeof(IEnumerable<DirectMessaging>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<DirectMessaging>, Error> GetDirectMessaging() =>
            HttpContext.GetUserId()
                       .Bind(_userService.GetDirectMessaging);

        /// <summary>
        ///     Fetches all invitations sent to the user.
        /// </summary>
        /// <returns>An MonadActionResult instance</returns>
        [HttpGet("invitations")]
        [ProducesResponseType(typeof(IEnumerable<Invitation>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<Invitation>, Error> GetInvitations() =>
            HttpContext.GetUserId()
                       .Bind(_userService.GetInvitations);

        /// <summary>
        ///     Updates the response of a user to a sent server invitation.
        /// </summary>
        /// <param name="invitation">The invitation instance to be used for the update</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpPut("invitation")]
        [ProducesResponseType(typeof(Invitation), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Invitation, Error> UpdateInvitation([FromBody] Invitation invitation) =>
            HttpContext.GetUserId()
                       .Bind(userId => _userService.ValidateInvitation(userId, invitation))
                       .Bind(_userService.UpdateInvitation);
    }
}
