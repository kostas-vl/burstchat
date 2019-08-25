using System;
using System.Collections.Generic;
using BurstChat.Api.Errors;
using BurstChat.Shared.Context;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Services.UserService;
using BurstChat.Shared.Schema.Users;
using BurstChat.Shared.Schema.Servers;
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
        /// <returns>An implementation instance of IActionResult</returns>
        [HttpGet]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Get()
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(_userService.Get);

            return this.UnwrapMonad(monad);
        }
        /// <summary>
        ///   This method will update the properties of a user based on the provided instance.
        /// </summary>
        /// <param name="user">The user instance to be user in the update</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Put([FromBody] User user)
        {
            var monad = _userService.Update(user);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will delete an existing user based on the provided id.
        /// </summary>
        /// <param name="id">The id of the user to be delete</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Delete(long id)
        {
            var monad = HttpContext
              .GetUserId()
              .Bind(_userService.Delete);

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will fetch all subscribed servers of a user based on the provided user id.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("subscriptions")]
        [ProducesResponseType(typeof(IEnumerable<Server>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetSubscribedServers()
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(_userService.GetSubscribedServers);

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will fetch all private groups that the user with the provided id is part of.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("groups")]
        [ProducesResponseType(typeof(IEnumerable<PrivateGroupMessage>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetPrivateGroups()
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(_userService.GetPrivateGroups);

            return this.UnwrapMonad(monad);
        }
    }
}
