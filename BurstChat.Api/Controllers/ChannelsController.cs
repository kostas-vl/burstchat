using System;
using System.Collections.Generic;
using BurstChat.Api.Errors;
using BurstChat.Api.Services.ChannelsService;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
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
        /// </summary>
        public ChannelsController(
            ILogger<ChannelsController> logger,
            IChannelsService channelsService
        )
        {
            _logger = logger;
            _channelsService = channelsService;
        }

        /// <summary>
        /// This method will fetch all any available information about a channel, if it exists,
        /// based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{channelId:int}")]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Get(int channelId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _channelsService.Get(userId, channelId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// This method will create a new channel based on the instance provided to an existing server
        /// that will be found based on the given server id.
        /// The server id in this request is provided as a query parameter.
        /// </summary>
        /// <param name="serverId">The id of the server the channel will be created</param>
        /// <param name="channel">The channel instance that will be created</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Post([FromBody] Channel channel, [FromQuery] int serverId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _channelsService.Insert(userId, serverId, channel));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// This method will update the properties of an existing channel based on the instance provided.
        /// </summary>
        /// <param name="channel">The channel instance to be updated</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Put([FromBody] Channel channel)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _channelsService.Update(userId, channel));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// This method will remove an existing channel based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the channel to be removed</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{channelId:int}")]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Delete(int channelId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _channelsService.Delete(userId, channelId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// This method will fetch all messages posted on a channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="lastMessageId">The id of the message to be the interval for the rest</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{channelId:int}/messages")]
        [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetMessages(int channelId, [FromQuery] long? lastMessageId = null)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _channelsService.GetMessages(userId, channelId, lastMessageId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// This method will add a new message to the channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="message">The message to be added</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("{channelId:int}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PostMessage(int channelId, [FromBody] Message message)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _channelsService.InsertMessage(userId, channelId, message));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// This method will update an existing message of a channel based on the provided
        /// instance.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut("{channelId:int}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PutMessage(int channelId, [FromBody] Message message)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _channelsService.UpdateMessage(userId, channelId, message));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// This method will delete an existing message from a channel.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{channelId:int}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult DeleteMessage(int channelId, [FromBody] long messageId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _channelsService.DeleteMessage(userId, channelId, messageId));

            return this.UnwrapMonad(monad);
        }
    }
}
