using System;
using System.Collections.Generic;
using BurstChat.Api.Errors;
using BurstChat.Api.Extensions;
using BurstChat.Api.Services.ChannelsService;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    ///   This class represents an ASPNET Core controller that exposes endpoints for
    ///   registering and posting messages to server channels.
    /// </summary>
    [ApiController]
    [Route("/api/channels")]
    [Produces("application/json")]
    public class ChannelsController : ControllerBase
    {
        private readonly ILogger<ChannelsController> _logger;
        private readonly IChannelsService _channelsService;

        /// <summary>
        ///   Executes any neccessary start up code for the controller.
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
        ///   This method will fetch all any available information about a channel, if it exists,
        ///   based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{channelId:int}")]
        [ProducesResponseType(typeof(Channel), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Get(int channelId)
        {
            var monad = _channelsService.Get(channelId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will create a new channel based on the instance provided to an existing server
        ///   that will be found based on the given server id.
        ///   The server id in this request is provided as a query parameter.
        /// </summary>
        /// <param name="serverId">The id of the server the channel will be created</param>
        /// <param name="channel">The channel instance that will be created</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Post(int serverId, [FromBody] Channel channel)
        {
            var monad = _channelsService.Insert(serverId, channel);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will update the properties of an existing channel based on the instance provided.
        /// </summary>
        /// <param name="channel">The channel instance to be updated</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Put([FromBody] Channel channel)
        {
            var monad = _channelsService.Update(channel);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will remove an existing channel based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the channel to be removed</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{channelId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Delete(int channelId)
        {
            var monad = _channelsService.Delete(channelId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will fetch all messages posted on a channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{channelId:int}/messages")]
        [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetMessages(int channelId)
        {
            var monad = _channelsService.GetMessages(channelId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will add a new message to the channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="message">The message to be added</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("{channelId:int}/messages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PostMessage(int channelId, [FromBody] Message message)
        {
            var monad = _channelsService.InsertMessage(channelId, message);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will update an existing message of a channel based on the provided
        ///   instance.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut("{channelId:int}/messages")] 
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PutMessage(int channelId, [FromBody] Message message)
        {
            var monad = _channelsService.UpdateMessage(channelId, message);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   This method will delete an existing message from a channel.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{channelId:int}/messages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult DeleteMessage(int channelId, [FromBody] Message message)
        {
            var monad = _channelsService.DeleteMessage(channelId, message);
            return this.UnwrapMonad(monad);
        }
    }
}
