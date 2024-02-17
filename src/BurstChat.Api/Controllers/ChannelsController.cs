using System;
using System.Collections.Generic;
using BurstChat.Api.ActionResults;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Services.ChannelsService;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/channels")]
[Produces("application/json")]
public class ChannelsController : ControllerBase
{
    private readonly ILogger<ChannelsController> _logger;
    private readonly IChannelsService _channelsService;

    public ChannelsController(
        ILogger<ChannelsController> logger,
        IChannelsService channelsService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
    }

    [HttpGet("{channelId:int}")]
    [ProducesResponseType(typeof(Channel), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Channel, Error> Get(int channelId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _channelsService.Get(userId, channelId));

    [HttpPost]
    [ProducesResponseType(typeof(Channel), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Channel, Error> Post([FromBody] Channel channel, [FromQuery] int serverId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _channelsService.Insert(userId, serverId, channel));

    [HttpPut]
    [ProducesResponseType(typeof(Channel), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Channel, Error> Put([FromBody] Channel channel) =>
        HttpContext.GetUserId()
                   .Bind(userId => _channelsService.Update(userId, channel));

    [HttpDelete("{channelId:int}")]
    [ProducesResponseType(typeof(Channel), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Channel, Error> Delete(int channelId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _channelsService.Delete(userId, channelId));

    [HttpGet("{channelId:int}/messages")]
    [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<IEnumerable<Message>, Error> GetMessages(int channelId,
                                                                      [FromQuery] string? searchTerm,
                                                                      [FromQuery] long? lastMessageId = null) =>
        HttpContext.GetUserId()
                   .Bind(userId => _channelsService.GetMessages(userId, channelId, searchTerm, lastMessageId));

    [HttpPost("{channelId:int}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Message, Error> PostMessage(int channelId, [FromBody] Message message) =>
        HttpContext.GetUserId()
                   .Bind(userId => _channelsService.InsertMessage(userId, channelId, message));

    [HttpPut("{channelId:int}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Message, Error> PutMessage(int channelId, [FromBody] Message message) =>
        HttpContext.GetUserId()
                   .Bind(userId => _channelsService.UpdateMessage(userId, channelId, message));

    [HttpDelete("{channelId:int}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public MonadActionResult<Message, Error> DeleteMessage(int channelId, [FromBody] long messageId) =>
        HttpContext.GetUserId()
                   .Bind(userId => _channelsService.DeleteMessage(userId, channelId, messageId));
}
