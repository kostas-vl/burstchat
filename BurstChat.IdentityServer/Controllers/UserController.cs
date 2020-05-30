using BurstChat.Application.Errors;
using BurstChat.Application.Extensions;
using BurstChat.Application.Models;
using BurstChat.Application.Services.UserService;
using BurstChat.Application.Services.ModelValidationService;
using BurstChat.Domain.Schema.Users;
using BurstChat.Domain.Schema.Servers;
using BurstChat.IdentityServer.Extensions;
using BurstChat.Infrastructure.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BurstChat.IdentityServer.Controllers
{
    /// <summary>
    /// This class represents an ASPNET Core controller that exposes endpoints for interacting with
    /// user data.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("connect")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IModelValidationService _modelValidationService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// </summary>
        public UserController(
            ILogger<UserController> logger,
            IModelValidationService modelValidationService,
            IEmailService emailService,
            IUserService userService
        )
        {
            _logger = logger;
            _modelValidationService = modelValidationService;
            _emailService = emailService;
            _userService = userService;
        }

        /// <summary>
        ///   This method will create a new user based on the parameters provided.
        /// </summary>
        /// <param name="registration">The registration parameters</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("register")]
        public IActionResult Post([FromBody] Registration registration)
        {
            var monad = _modelValidationService
                .ValidateRegistration(registration)
                .Bind(r => _userService.Insert(r.AlphaInvitationCode,
                                               r.Email,
                                               r.Name,
                                               r.Password));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will create a new one time password for a user registered with the provided email
        ///   and it will be sent to the said mail.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("password/reset")]
        public async Task<IActionResult> IssueOneTimePassword([FromBody] string email)
        {
            var monad = await _userService
                .IssueOneTimePassword(email)
                .BindAsync(async oneTimePass =>
                {
                    return await _emailService.SendOneTimePasswordAsync(email, oneTimePass);
                });

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will change the password of a user based on the provided parameters.
        /// </summary>
        /// <param name="changePassword">The change password properties</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("password/change")]
        public IActionResult ChangePassword([FromBody] ChangePassword changePassword)
        {
            var monad = _modelValidationService
                .ValidateChangePassword(changePassword)
                .Bind(c => _userService.ChangePassword(c.Email, c.OneTimePassword, c.NewPassword));

            return this.UnwrapMonad(monad);
        }
    }
}
