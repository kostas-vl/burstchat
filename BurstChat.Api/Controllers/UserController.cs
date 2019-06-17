using System;
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
        private readonly BurstChatContext _burstChatContext;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// </summary>
        public UserController(
            ILogger<UserController> logger,
            BurstChatContext burstChatContext
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
        }

        /// <summary>
        /// This method fetches information about a user based on the provided id.
        /// </summary>
        /// <param name="id">The id of the target user.</param>
        /// <returns>An implementation instance of IActionResult</returns>
        [HttpGet("single/{id:long}")]
        public IActionResult Get(long id)
        {
            return Ok();
        }
    }
}
