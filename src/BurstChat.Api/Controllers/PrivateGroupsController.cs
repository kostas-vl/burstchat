using System;
using System.Collections.Generic;
using BurstChat.Application.Services.PrivateGroupsService;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using BurstChat.Infrastructure.Errors;
using BurstChat.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.Controllers;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("/api/groups")]
public class PrivateGroupsController : ControllerBase
{
    private readonly IPrivateGroupsService _privateGroupMessagingService;

    public PrivateGroupsController(IPrivateGroupsService privateGroupsService)
    {
        _privateGroupMessagingService =
            privateGroupsService ?? throw new ArgumentNullException(nameof(privateGroupsService));
    }

    [HttpGet("{groupId:long}")]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Get(long groupId) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.Get(userId, groupId))
            .Into();

    [HttpPost]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Post([FromBody] string groupName) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.Insert(userId, groupName))
            .Into();

    [HttpPost("{groupName}")]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Post(string groupName, [FromBody] IEnumerable<long> userIds) =>
        HttpContext
            .GetUserId()
            .And(userId =>
                _privateGroupMessagingService
                    .Insert(userId, groupName)
                    .And(privateGroup =>
                        _privateGroupMessagingService.InsertUsers(userId, privateGroup.Id, userIds)
                    )
            )
            .Into();

    [HttpDelete("{groupId:long}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Delete(long groupId) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.Delete(userId, groupId))
            .Into();

    [HttpPost("{groupId:long}/user")]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult PostUser(long groupId, [FromBody] long newUserId) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.InsertUser(userId, groupId, newUserId))
            .Into();

    [HttpDelete("{groupId:long}/user")]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult DeleteUser(long groupId, [FromBody] long targetUserId) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.DeleteUser(userId, groupId, targetUserId))
            .Into();

    [HttpGet("{groupId:long}/messages")]
    [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetMessages(long groupId) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.GetMessages(userId, groupId))
            .Into();

    [HttpPost("{groupId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult PostMessage(long groupId, [FromBody] Message message) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.InsertMessage(userId, groupId, message))
            .Into();

    [HttpPut("{groupId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult PutMessage(long groupId, [FromBody] Message message) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.UpdateMessage(userId, groupId, message))
            .Into();

    [HttpDelete("{groupId:long}/messages")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult DeleteMessage(long groupId, [FromBody] long messageId) =>
        HttpContext
            .GetUserId()
            .And(userId => _privateGroupMessagingService.DeleteMessage(userId, groupId, messageId))
            .Into();
}
