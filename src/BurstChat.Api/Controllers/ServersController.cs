using System;
using System.Collections.Generic;
using BurstChat.Application.Services.ServersService;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using BurstChat.Infrastructure.Errors;
using BurstChat.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/servers")]
[Produces("application/json")]
public class ServersController : ControllerBase
{
    private readonly ILogger<ServersController> _logger;
    private readonly IServersService _serversService;

    public ServersController(ILogger<ServersController> logger, IServersService serversService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serversService = serversService ?? throw new ArgumentNullException(nameof(serversService));
    }

    [HttpGet("{serverId:int}")]
    [ProducesResponseType(typeof(Server), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Get(int serverId) =>
        HttpContext.GetUserId().And(userId => _serversService.Get(userId, serverId)).Into();

    [HttpPost]
    [ProducesResponseType(typeof(Server), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Post([FromBody] Server server) =>
        HttpContext.GetUserId().And(userId => _serversService.Insert(userId, server)).Into();

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Put([FromBody] Server server) =>
        HttpContext.GetUserId().And(userId => _serversService.Update(userId, server)).Into();

    [HttpDelete("{serverId:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Delete(int serverId) =>
        HttpContext.GetUserId().And(userId => _serversService.Delete(userId, serverId)).Into();

    [HttpGet("{serverId:int}/subscribedUsers")]
    [ProducesResponseType(typeof(IEnumerable<User>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetSubscribedUsers(int serverId) =>
        HttpContext
            .GetUserId()
            .And(userId => _serversService.GetSubscribedUsers(userId, serverId))
            .Into();

    [HttpDelete("{serverId:int}/subscriptions")]
    [ProducesResponseType(typeof(Subscription), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult DeleteSubscription(int serverId, [FromBody] Subscription subscription) =>
        HttpContext
            .GetUserId()
            .And(userId => _serversService.DeleteSubscription(userId, serverId, subscription))
            .Into();

    [HttpGet("{serverId:int}/invitations")]
    [ProducesResponseType(typeof(IEnumerable<Invitation>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetInvitations(int serverId) =>
        HttpContext
            .GetUserId()
            .And(userId => _serversService.GetInvitations(userId, serverId))
            .Into();

    [HttpPost("{serverId:int}/invitation")]
    [ProducesResponseType(typeof(Invitation), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult InsertInvitation(int serverId, [FromBody] string username) =>
        HttpContext
            .GetUserId()
            .And(userId => _serversService.InsertInvitation(userId, serverId, username))
            .Into();
}
