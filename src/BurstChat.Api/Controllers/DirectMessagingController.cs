using System;
using System.Collections.Generic;
using BurstChat.Api.ActionResults;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.DirectMessagingService;
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
        _directMessagingService = directMessagingService
            ?? throw new ArgumentNullException(nameof(directMessagingService));
    }

    [HttpGet("{directMessagingId:long}")]
    [ProducesResponseType(typeof(DirectMessaging), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<DirectMessaging, Error> Get(long directMessagingId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _directMessagingService.Get(userId, directMessagingId));

    [HttpGet]
    [ProducesResponseType(typeof(DirectMessaging), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<DirectMessaging, Error> Get([FromQuery] long firstParticipantId, [FromQuery] long secondParticipantId)
    {
        var monad = HttpContext
            .GetUserId()
            .Bind(userId => _directMessagingService.Get(userId, firstParticipantId, secondParticipantId));

        if (monad is Failure<DirectMessaging, Error>)
        {
            var directMessaging = new DirectMessaging
            {
                FirstParticipantUserId = firstParticipantId,
                SecondParticipantUserId = secondParticipantId
            };
            return Post(directMessaging);
        }

        return monad;
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(IEnumerable<User>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<User?>, Error> GetUsers() =>
        HttpContext.GetUserId()
                   .Bind(_directMessagingService.GetUsers);

    [HttpPost]
    [ProducesResponseType(typeof(DirectMessaging), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<DirectMessaging, Error> Post([FromBody] DirectMessaging directMessaging) =>
        HttpContext.GetUserId()
                   .Bind(userId => _directMessagingService.Insert(userId,
                                                                  directMessaging.FirstParticipantUserId,
                                                                  directMessaging.SecondParticipantUserId));

    [HttpDelete("{directMessagingId:long}")]
    [ProducesResponseType(typeof(DirectMessaging), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<DirectMessaging, Error> Delete(long directMessagingId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _directMessagingService.Delete(userId, directMessagingId));

    [HttpGet("{directMessagingId:long}/messages")]
    [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<Message>, Error> GetMessages(long directMessagingId,
                                                                      [FromQuery] string? searchTerm,
                                                                      [FromQuery] long? lastMessageId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _directMessagingService.GetMessages(userId,
                                                                       directMessagingId,
                                                                       searchTerm,
                                                                       lastMessageId));

    [HttpPost("{directMessagingId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Message, Error> PostMessage(long directMessagingId, [FromBody] Message message) =>
        HttpContext.GetUserId()
                   .Bind(userId => _directMessagingService.InsertMessage(userId, directMessagingId, message));

    [HttpPut("{directMessagingId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Message, Error> PutMessage(long directMessagingId, [FromBody] Message message) =>
        HttpContext.GetUserId()
                   .Bind(userId => _directMessagingService.UpdateMessage(userId, directMessagingId, message));

    [HttpDelete("{directMessagingId:long}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Message, Error> DeleteMessage(long directMessagingId, [FromBody] long messageId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _directMessagingService.DeleteMessage(userId, directMessagingId, messageId));
}
