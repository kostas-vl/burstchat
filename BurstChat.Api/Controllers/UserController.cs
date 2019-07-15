using System;
using BurstChat.Api.Errors;
using BurstChat.Api.Extensions;
using BurstChat.Api.Services.UserService;
using BurstChat.Shared.Context;
using BurstChat.Shared.Schema.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    /// This class represents an ASPNET Core controller that exposes endpoints for interacting with
    /// user data.
    /// </summary>
    [ApiController]
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
        /// This method fetches information about a user based on the provided id.
        /// </summary>
        /// <param name="id">The id of the target user.</param>
        /// <returns>An implementation instance of IActionResult</returns>
        [HttpGet("single/{id:long}")]
        public IActionResult Get(long id)
        {
            try
            {
                var monad = _userService.Select(id);
                return this.UnwrapMonad(monad);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return BadRequest(SystemErrors.Exception());
            }
        }
    }
}
