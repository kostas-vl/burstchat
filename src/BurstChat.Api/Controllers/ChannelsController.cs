using System;
using System.Collections.Generic;
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
    public IActionResult Get(int channelId) =>
        HttpContext
            .GetUserId()
            .And(userId => _channelsService.Get(userId, channelId))
            .Into();

    [HttpPost]
    [ProducesResponseType(typeof(Channel), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Post([FromBody] Channel channel, [FromQuery] int serverId) =>
        HttpContext
            .GetUserId()
            .And(userId => _channelsService.Insert(userId, serverId, channel))
            .Into();

    [HttpPut]
    [ProducesResponseType(typeof(Channel), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Put([FromBody] Channel channel) =>
        HttpContext
            .GetUserId()
            .And(userId => _channelsService.Update(userId, channel))
            .Into();

    [HttpDelete("{channelId:int}")]
    [ProducesResponseType(typeof(Channel), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult Delete(int channelId) =>
        HttpContext
            .GetUserId()
            .And(userId => _channelsService.Delete(userId, channelId))
            .Into();

    [HttpGet("{channelId:int}/messages")]
    [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult GetMessages(
        int channelId,
        [FromQuery] string? searchTerm,
        [FromQuery] long? lastMessageId = null) =>
        HttpContext
            .GetUserId()
            .And(userId => _channelsService.GetMessages(userId, channelId, searchTerm, lastMessageId))
            .Into();

    [HttpPost("{channelId:int}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult PostMessage(int channelId, [FromBody] Message message) =>
        HttpContext
            .GetUserId()
            .And(userId => _channelsService.InsertMessage(userId, channelId, message))
            .Into();

    [HttpPut("{channelId:int}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult PutMessage(int channelId, [FromBody] Message message) =>
        HttpContext
            .GetUserId()
            .And(userId => _channelsService.UpdateMessage(userId, channelId, message))
            .Into();

    [HttpDelete("{channelId:int}/messages")]
    [ProducesResponseType(typeof(Message), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public IActionResult DeleteMessage(int channelId, [FromBody] long messageId) =>
        HttpContext
            .GetUserId()
            .And(userId => _channelsService.DeleteMessage(userId, channelId, messageId))
            .Into();
}
