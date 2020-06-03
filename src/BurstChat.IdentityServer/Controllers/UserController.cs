using System;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Models;
using BurstChat.Application.Services.UserService;
using BurstChat.Application.Services.ModelValidationService;
using BurstChat.IdentityServer.ActionResults;
using BurstChat.IdentityServer.Extensions;
using BurstChat.Infrastructure.Services.AsteriskService;
using BurstChat.Infrastructure.Services.EmailService;
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
        private readonly IAsteriskService _asteriskService;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        ///
        /// Exceptions
        ///     ArgumentNullException: when any of the parameters is null.
        /// </summary>
        public UserController(
            ILogger<UserController> logger,
            IModelValidationService modelValidationService,
            IEmailService emailService,
            IUserService userService,
            IAsteriskService asteriskService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _modelValidationService = modelValidationService ?? throw new ArgumentNullException(nameof(modelValidationService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _asteriskService = asteriskService ?? throw new ArgumentNullException(nameof(asteriskService));
        }

        /// <summary>
        /// This method will create a new user based on the parameters provided.
        /// </summary>
        /// <param name="registration">The registration parameters</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost("register")]
        public async Task<MonadActionResult<Unit, Error>> Post([FromBody] Registration registration) =>
            await _modelValidationService.ValidateRegistration(registration)
                                         .Bind(r => _userService.Insert(r.AlphaInvitationCode,
                                                                        r.Email,
                                                                        r.Name,
                                                                        r.Password))
                                         .BindAsync(async user =>
                                         {
                                              var endpoint = Guid.Parse(user.Sip.Username);
                                              var password = Guid.NewGuid();
                                              var monad = await _asteriskService.PostAsync(endpoint, password);
                                              return monad.Attach(_ => Unit.New());
                                         });

        /// <summary>
        /// This method will create a new one time password for a user registered with the provided email
        /// and it will be sent to the said mail.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpPost("password/reset")]
        public async Task<MonadActionResult<Unit, Error>> IssueOneTimePassword([FromBody] string email) =>
            await _userService.IssueOneTimePassword(email)
                              .BindAsync(async oneTimePass =>
                              {
                                  return await _emailService.SendOneTimePasswordAsync(email, oneTimePass);
                              });

        /// <summary>
        ///   This method will change the password of a user based on the provided parameters.
        /// </summary>
        /// <param name="changePassword">The change password properties</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpPost("password/change")]
        public MonadActionResult<Unit, Error> ChangePassword([FromBody] ChangePassword changePassword) =>
            _modelValidationService.ValidateChangePassword(changePassword)
                                   .Bind(c => _userService.ChangePassword(c.Email, c.OneTimePassword, c.NewPassword));
    }
}
