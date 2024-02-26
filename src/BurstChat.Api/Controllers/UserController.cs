using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Models;
using BurstChat.Application.Services.UserService;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Infrastructure.Services.AsteriskService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers;

[ApiController]
[Authorize]
[Produces("application/json")]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IAsteriskService _asteriskService;

    public UserController(
        ILogger<UserController> logger,
        IUserService userService,
        IAsteriskService asteriskService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _asteriskService =
            asteriskService ?? throw new ArgumentNullException(nameof(asteriskService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Get() => HttpContext.GetUserId().And(_userService.Get).Into();

    [HttpGet("{userId:long}")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Get(long userId) =>
        HttpContext.GetUserId().And(_ => _userService.Get(userId)).Into();

    [HttpPut]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Put([FromBody] User user) => _userService.Update(user).Into();

    [HttpDelete("{id:long}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Delete(long id) => HttpContext.GetUserId().And(_userService.Delete).Into();

    [HttpGet("subscriptions")]
    [ProducesResponseType(typeof(IEnumerable<Server>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetSubscriptions() =>
        HttpContext.GetUserId().And(_userService.GetSubscriptions).Into();

    [HttpGet("groups")]
    [ProducesResponseType(typeof(IEnumerable<PrivateGroup>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetPrivateGroups() =>
        HttpContext.GetUserId().And(_userService.GetPrivateGroups).Into();

    [HttpGet("direct")]
    [ProducesResponseType(typeof(IEnumerable<DirectMessaging>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetDirectMessaging() =>
        HttpContext.GetUserId().And(_userService.GetDirectMessaging).Into();

    [HttpGet("invitations")]
    [ProducesResponseType(typeof(IEnumerable<Invitation>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetInvitations() =>
        HttpContext.GetUserId().And(_userService.GetInvitations).Into();

    [HttpPut("invitation")]
    [ProducesResponseType(typeof(Invitation), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult UpdateInvitation([FromBody] UpdateInvitation invitation) =>
        HttpContext
            .GetUserId()
            .And(userId => _userService.UpdateInvitation(userId, invitation))
            .Into();

    [HttpGet("sip")]
    [ProducesResponseType(typeof(AsteriskEndpoint), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public async Task<IActionResult> GetSipEndpoint() =>
        await HttpContext
            .GetUserId()
            .Map(userId => userId.ToString())
            .AndAsync(_asteriskService.GetAsync)
            .IntoAsync();
}
