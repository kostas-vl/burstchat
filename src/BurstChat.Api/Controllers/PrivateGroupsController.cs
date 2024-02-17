using System;
using System.Collections.Generic;
using BurstChat.Api.ActionResults;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.PrivateGroupsService;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
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
        _privateGroupMessagingService = privateGroupsService
            ?? throw new ArgumentNullException(nameof(privateGroupsService));
    }

    [HttpGet("{groupId:long}")]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<PrivateGroup, Error> Get(long groupId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.Get(userId, groupId));

    [HttpPost]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<PrivateGroup, Error> Post([FromBody] string groupName) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.Insert(userId, groupName));

    [HttpPost("{groupName}")]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<PrivateGroup, Error> Post(string groupName, [FromBody] IEnumerable<long> userIds) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.Insert(userId, groupName))
                   .Bind(privateGroup => HttpContext.GetUserId()
                                                    .Bind(userId => _privateGroupMessagingService.InsertUsers(userId, privateGroup.Id, userIds)));

    [HttpDelete("{groupId:long}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Unit, Error> Delete(long groupId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.Delete(userId, groupId));

    [HttpPost("{groupId:long}/user")]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<PrivateGroup, Error> PostUser(long groupId, [FromBody] long newUserId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.InsertUser(userId, groupId, newUserId));

    [HttpDelete("{groupId:long}/user")]
    [ProducesResponseType(typeof(PrivateGroup), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<PrivateGroup, Error> DeleteUser(long groupId, [FromBody] long targetUserId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.DeleteUser(userId, groupId, targetUserId));

    [HttpGet("{groupId:long}/messages")]
    [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<Message>, Error> GetMessages(long groupId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.GetMessages(userId, groupId));

    [HttpPost("{groupId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Message, Error> PostMessage(long groupId, [FromBody] Message message) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.InsertMessage(userId, groupId, message));

    [HttpPut("{groupId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Message, Error> PutMessage(long groupId, [FromBody] Message message) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.UpdateMessage(userId, groupId, message));

    [HttpDelete("{groupId:long}/messages")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Unit, Error> DeleteMessage(long groupId, [FromBody] long messageId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _privateGroupMessagingService.DeleteMessage(userId, groupId, messageId));
}
