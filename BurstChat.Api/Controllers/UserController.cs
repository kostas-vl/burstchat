using System;
using BurstChat.Api.Errors;
using BurstChat.Api.Extensions;
using BurstChat.Api.Models;
using BurstChat.Api.Services.ModelValidationService;
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
        private readonly IModelValidationService _modelValidationService;
        private readonly IUserService _userService;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// </summary>
        public UserController(
            ILogger<UserController> logger,
            IModelValidationService modelValidationService,
            IUserService userService
        )
        {
            _logger = logger;
            _modelValidationService = modelValidationService;
            _userService = userService;
        }

        /// <summary>
        /// This method fetches information about a user based on the provided id.
        /// </summary>
        /// <param name="id">The id of the target user.</param>
        /// <returns>An implementation instance of IActionResult</returns>
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            var monad = _userService.Get(id);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will create a new user based on the parameters provided.
        /// </summary>
        /// <param name="registration">The registration parameters</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost]
        public IActionResult Post([FromBody] Registration registration)
        {
            var monad = _modelValidationService
                .ValidateRegistration(registration)
                .Bind(r => _userService.Insert(r.Email, r.Password));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will update the properties of a user based on the provided instance.
        /// </summary>
        /// <param name="user">The user instance to be user in the update</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut]
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
        public IActionResult Delete(long id)
        {
            var monad = _userService.Delete(id);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will use the credentials instance provided and check if both the email
        ///   and password belong to a user.
        /// </summary>
        /// <param name="credentials">The credentials to be checked</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("validate")]
        public IActionResult Validate([FromBody] Credentials credentials)
        {
            var email = credentials?.Email;
            var password = credentials?.Password;
            var monad = _userService.Validate(email, password);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will create a new one time password for a user registered with the provided email
        ///   and it will be sent to the said mail.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>An IActionResult instance</returns>
        public IActionResult IssueOneTimePassword([FromBody] string email)
        {
            var monad = _userService.IssueOneTimePassword(email);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will change the password of a user based on the provided parameters.
        /// </summary>
        /// <param name="changePassword">The change password properties</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("/password/change")]
        public IActionResult ChangePassword([FromBody] ChangePassword changePassword)
        {
            var monad = _modelValidationService
                .ValidateChangePassword(changePassword)
                .Bind(c => _userService.ChangePassword(c.OneTimePassword, c.NewPassword));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will fetch all subscribed servers of a user based on the provided user id.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{userId:long}/subscriptions")]
        public IActionResult GetSubscribedServers(long userId)
        {
            var monad = _userService.GetSubscribedServers(userId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will fetch all private groups that the user with the provided id is part of.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{userId:long}/groups")]
        public IActionResult GetPrivateGroups(long userId)
        {
            var monad = _userService.GetPrivateGroups(userId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will fetch all available messages of a group based on the provided id. Also a user id
        ///   will be provided that will be used in the search of the group.
        /// </summary>
        /// <param name="userId">The id of a user that is a member of the group</param>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{userId:long}/group/{groupId:long}/messages")]
        public IActionResult GetPrivateGroupMessages(long userId, long groupId)
        {
            var monad = _userService.GetPrivateGroupMessages(userId, groupId);
            return this.UnwrapMonad(monad);
        }
    }
}
