using System;
using System.Collections.Generic;
using BurstChat.Api.ActionResults;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.ServersService;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
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

    public ServersController(
        ILogger<ServersController> logger,
        IServersService serversService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serversService = serversService ?? throw new ArgumentNullException(nameof(serversService));
    }

    [HttpGet("{serverId:int}")]
    [ProducesResponseType(typeof(Server), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Server, Error> Get(int serverId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _serversService.Get(userId, serverId));

    [HttpPost]
    [ProducesResponseType(typeof(Server), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Server, Error> Post([FromBody] Server server) =>
        HttpContext.GetUserId()
                   .Bind(userId => _serversService.Insert(userId, server));

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Server, Error> Put([FromBody] Server server) =>
        HttpContext.GetUserId()
                   .Bind(userId => _serversService.Update(userId, server));

    [HttpDelete("{serverId:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Unit, Error> Delete(int serverId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _serversService.Delete(userId, serverId));

    [HttpGet("{serverId:int}/subscribedUsers")]
    [ProducesResponseType(typeof(IEnumerable<User>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<User>, Error> GetSubscribedUsers(int serverId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _serversService.GetSubscribedUsers(userId, serverId));

    [HttpDelete("{serverId:int}/subscriptions")]
    [ProducesResponseType(typeof(Subscription), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Subscription, Error> DeleteSubscription(int serverId, [FromBody] Subscription subscription) =>
        HttpContext.GetUserId()
                   .Bind(userId => _serversService.DeleteSubscription(userId, serverId, subscription));

    [HttpGet("{serverId:int}/invitations")]
    [ProducesResponseType(typeof(IEnumerable<Invitation>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<Invitation>, Error> GetInvitations(int serverId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _serversService.GetInvitations(userId, serverId));

    [HttpPost("{serverId:int}/invitation")]
    [ProducesResponseType(typeof(Invitation), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Invitation, Error> InsertInvitation(int serverId, [FromBody] string username) =>
        HttpContext.GetUserId()
                   .Bind(userId => _serversService.InsertInvitation(userId, serverId, username));
}
