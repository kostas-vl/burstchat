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

namespace BurstChat.Api.Controllers
{
    /// <summary>
    /// This class represents an ASPNET Core controller that exposes endpoints for
    /// registering and posting messages to server channels.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/channels")]
    [Produces("application/json")]
    public class ChannelsController : ControllerBase
    {
        private readonly ILogger<ChannelsController> _logger;
        private readonly IChannelsService _channelsService;

        /// <summary>
        /// Executes any neccessary start up code for the controller.
        ///
        /// Exceptions:
        ///     ArgumentNullException: When any parameter is null.
        /// </summary>
        public ChannelsController(
            ILogger<ChannelsController> logger,
            IChannelsService channelsService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
        }

        /// <summary>
        /// This method will fetch all any available information about a channel, if it exists,
        /// based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("{channelId:int}")]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Channel, Error> Get(int channelId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _channelsService.Get(userId, channelId));

        /// <summary>
        /// This method will create a new channel based on the instance provided to an existing server
        /// that will be found based on the given server id.
        /// The server id in this request is provided as a query parameter.
        /// </summary>
        /// <param name="serverId">The id of the server the channel will be created</param>
        /// <param name="channel">The channel instance that will be created</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Channel, Error> Post([FromBody] Channel channel, [FromQuery] int serverId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _channelsService.Insert(userId, serverId, channel));

        /// <summary>
        /// This method will update the properties of an existing channel based on the instance provided.
        /// </summary>
        /// <param name="channel">The channel instance to be updated</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPut]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Channel, Error> Put([FromBody] Channel channel) =>
            HttpContext.GetUserId()
                       .Bind(userId => _channelsService.Update(userId, channel));

        /// <summary>
        /// This method will remove an existing channel based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the channel to be removed</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpDelete("{channelId:int}")]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Channel, Error> Delete(int channelId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _channelsService.Delete(userId, channelId));

        /// <summary>
        /// This method will fetch all messages posted on a channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="searchTerm">The term that needs to be present to all returned messaged</param>
        /// <param name="lastMessageId">The id of the message to be the interval for the rest</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("{channelId:int}/messages")]
        [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<Message>, Error> GetMessages(int channelId,
                                                                          [FromQuery] string? searchTerm,
                                                                          [FromQuery] long? lastMessageId = null) =>
            HttpContext.GetUserId()
                       .Bind(userId => _channelsService.GetMessages(userId, channelId, searchTerm, lastMessageId));

        /// <summary>
        /// This method will add a new message to the channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="message">The message to be added</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost("{channelId:int}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Message, Error> PostMessage(int channelId, [FromBody] Message message) =>
            HttpContext.GetUserId()
                       .Bind(userId => _channelsService.InsertMessage(userId, channelId, message));

        /// <summary>
        /// This method will update an existing message of a channel based on the provided
        /// instance.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPut("{channelId:int}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Message, Error> PutMessage(int channelId, [FromBody] Message message) =>
            HttpContext.GetUserId()
                       .Bind(userId => _channelsService.UpdateMessage(userId, channelId, message));

        /// <summary>
        /// This method will delete an existing message from a channel.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpDelete("{channelId:int}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Message, Error> DeleteMessage(int channelId, [FromBody] long messageId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _channelsService.DeleteMessage(userId, channelId, messageId));
    }
}
