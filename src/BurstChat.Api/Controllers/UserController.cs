using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Api.ActionResults;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Models;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.UserService;
using BurstChat.Domain.Schema.Users;
using BurstChat.Domain.Schema.Servers;
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
        _asteriskService = asteriskService ?? throw new ArgumentNullException(nameof(asteriskService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<User, Error> Get() =>
        HttpContext.GetUserId()
                   .Bind(_userService.Get);

    [HttpGet("{userId:long}")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<User, Error> Get(long userId) =>
        HttpContext.GetUserId()
                   .Bind(_ => _userService.Get(userId));

    [HttpPut]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<User, Error> Put([FromBody] User user) =>
        _userService.Update(user);

    [HttpDelete("{id:long}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Unit, Error> Delete(long id) =>
        HttpContext.GetUserId()
                   .Bind(_userService.Delete);

    [HttpGet("subscriptions")]
    [ProducesResponseType(typeof(IEnumerable<Server>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<Server>, Error> GetSubscriptions() =>
        HttpContext.GetUserId()
                   .Bind(_userService.GetSubscriptions);

    [HttpGet("groups")]
    [ProducesResponseType(typeof(IEnumerable<PrivateGroup>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<PrivateGroup>, Error> GetPrivateGroups() =>
        HttpContext.GetUserId()
                   .Bind(_userService.GetPrivateGroups);

    [HttpGet("direct")]
    [ProducesResponseType(typeof(IEnumerable<DirectMessaging>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<DirectMessaging>, Error> GetDirectMessaging() =>
        HttpContext.GetUserId()
                   .Bind(_userService.GetDirectMessaging);

    [HttpGet("invitations")]
    [ProducesResponseType(typeof(IEnumerable<Invitation>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<Invitation>, Error> GetInvitations() =>
        HttpContext.GetUserId()
                   .Bind(_userService.GetInvitations);

    [HttpPut("invitation")]
    [ProducesResponseType(typeof(Invitation), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Invitation, Error> UpdateInvitation([FromBody] UpdateInvitation invitation) =>
        HttpContext.GetUserId()
                   .Bind(userId => _userService.UpdateInvitation(userId, invitation));

    [HttpGet("sip")]
    [ProducesResponseType(typeof(AsteriskEndpoint), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public async Task<MonadActionResult<AsteriskEndpoint, Error>> GetSipEndpoint() =>
        await HttpContext.GetUserId()
                         .BindAsync(async userId => await _asteriskService.GetAsync(userId.ToString()));
}
