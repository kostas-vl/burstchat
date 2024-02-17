using System;
using System.Collections.Generic;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Services.DirectMessagingService;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.Controllers;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("/api/direct")]
public class DirectMessagingController : ControllerBase
{
    private readonly IDirectMessagingService _directMessagingService;

    public DirectMessagingController(IDirectMessagingService directMessagingService)
    {
        _directMessagingService =
            directMessagingService
            ?? throw new ArgumentNullException(nameof(directMessagingService));
    }

    [HttpGet("{directMessagingId:long}")]
    [ProducesResponseType(typeof(DirectMessaging), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Get(long directMessagingId) =>
        HttpContext
            .GetUserId()
            .And(userId => _directMessagingService.Get(userId, directMessagingId))
            .Into();

    [HttpGet]
    [ProducesResponseType(typeof(DirectMessaging), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Get(
        [FromQuery] long firstParticipantId,
        [FromQuery] long secondParticipantId
    )
    {
        var res = HttpContext
            .GetUserId()
            .And(userId =>
                _directMessagingService.Get(userId, firstParticipantId, secondParticipantId)
            );

        if (res.IsErr)
        {
            var directMessaging = new DirectMessaging
            {
                FirstParticipantUserId = firstParticipantId,
                SecondParticipantUserId = secondParticipantId
            };
            return Post(directMessaging);
        }

        return res.Into();
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(IEnumerable<User>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetUsers() =>
        HttpContext.GetUserId().And(_directMessagingService.GetUsers).Into();

    [HttpPost]
    [ProducesResponseType(typeof(DirectMessaging), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Post([FromBody] DirectMessaging directMessaging) =>
        HttpContext
            .GetUserId()
            .And(userId =>
                _directMessagingService.Insert(
                    userId,
                    directMessaging.FirstParticipantUserId,
                    directMessaging.SecondParticipantUserId
                )
            )
            .Into();

    [HttpDelete("{directMessagingId:long}")]
    [ProducesResponseType(typeof(DirectMessaging), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Delete(long directMessagingId) =>
        HttpContext
            .GetUserId()
            .And(userId => _directMessagingService.Delete(userId, directMessagingId))
            .Into();

    [HttpGet("{directMessagingId:long}/messages")]
    [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetMessages(
        long directMessagingId,
        [FromQuery] string? searchTerm,
        [FromQuery] long? lastMessageId
    ) =>
        HttpContext
            .GetUserId()
            .And(userId =>
                _directMessagingService.GetMessages(
                    userId,
                    directMessagingId,
                    searchTerm,
                    lastMessageId
                )
            )
            .Into();

    [HttpPost("{directMessagingId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult PostMessage(long directMessagingId, [FromBody] Message message) =>
        HttpContext
            .GetUserId()
            .And(userId =>
                _directMessagingService.InsertMessage(userId, directMessagingId, message)
            )
            .Into();

    [HttpPut("{directMessagingId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult PutMessage(long directMessagingId, [FromBody] Message message) =>
        HttpContext
            .GetUserId()
            .And(userId =>
                _directMessagingService.UpdateMessage(userId, directMessagingId, message)
            )
            .Into();

    [HttpDelete("{directMessagingId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult DeleteMessage(long directMessagingId, [FromBody] long messageId) =>
        HttpContext
            .GetUserId()
            .And(userId =>
                _directMessagingService.DeleteMessage(userId, directMessagingId, messageId)
            )
            .Into();
}
