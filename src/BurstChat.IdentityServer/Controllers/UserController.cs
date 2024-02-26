using System;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Models;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.ModelValidationService;
using BurstChat.Application.Services.UserService;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Infrastructure.Services.AsteriskService;
using BurstChat.Infrastructure.Services.EmailService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.IdentityServer.Controllers;

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

    public UserController(
        ILogger<UserController> logger,
        IModelValidationService modelValidationService,
        IEmailService emailService,
        IUserService userService,
        IAsteriskService asteriskService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _modelValidationService =
            modelValidationService
            ?? throw new ArgumentNullException(nameof(modelValidationService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _asteriskService =
            asteriskService ?? throw new ArgumentNullException(nameof(asteriskService));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Post([FromBody] Registration registration) =>
        await _modelValidationService
            .ValidateRegistration(registration)
            .And(r => _userService.Insert(r.AlphaInvitationCode, r.Email, r.Name, r.Password))
            .AndAsync(user =>
            {
                var endpoint = user.Id.ToString();
                var password = Guid.NewGuid();
                return _asteriskService.PostAsync(endpoint, password);
            })
            .MapAsync(_ => Unit.Instance)
            .IntoAsync();

    [HttpPost("password/reset")]
    public async Task<IActionResult> IssueOneTimePassword([FromBody] string email) =>
        await _userService
            .IssueOneTimePassword(email)
            .AndAsync(async oneTimePass =>
            {
                return await _emailService.SendOneTimePasswordAsync(email, oneTimePass);
            })
            .IntoAsync();

    [HttpPost("password/change")]
    public IActionResult ChangePassword([FromBody] ChangePassword changePassword) =>
        _modelValidationService
            .ValidateChangePassword(changePassword)
            .And(c => _userService.ChangePassword(c.Email, c.OneTimePassword, c.NewPassword))
            .Into();
}
