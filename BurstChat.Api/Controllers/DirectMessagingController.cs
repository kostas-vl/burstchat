using System;
using System.Collections.Generic;
using System.Linq;
using BurstChat.Api.Errors;
using BurstChat.Api.Services.DirectMessagingService;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    /// This class represents an ASPNET Core controller that exposes endpoints for interacting with
    /// data of direct messages.
    /// </summary>
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("/api/direct")]
    public class DirectMessagingController : ControllerBase
    {
        private readonly IDirectMessagingService _directMessagingService;

        /// <summary>
        /// Creates a new instance of DirectMessagingController.
        /// </summary>
        public DirectMessagingController(IDirectMessagingService directMessagingService)
        {
            _directMessagingService = directMessagingService;
        }
        
        /// <summary>
        /// Fetches all available information about the direct messages of a user.
        /// </summary>
        /// <param name="directMessagingId">The id of the direct messages</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{directMessagingId:long}")]
        [ProducesResponseType(typeof(DirectMessaging), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Get(long directMessagingId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _directMessagingService.Get(userId, directMessagingId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// Fetches all available information about the direct messages between two users.
        /// </summary>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the second participant</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet]
        [ProducesResponseType(typeof(DirectMessaging), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Get([FromQuery] long firstParticipantId, [FromQuery] long secondParticipantId)
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
            else
                return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// Fetches all users that the authenticated user has sent a direct messages.
        /// </summary>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("users")]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetUsers()
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(_directMessagingService.GetUsers);

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// Creates a new direct messaging association between 2 users.
        /// </summary>
        /// <param name="directMessaging">The direct messaging instance to be created</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DirectMessaging), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Post([FromBody] DirectMessaging directMessaging)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _directMessagingService.Insert(userId, directMessaging.FirstParticipantUserId, directMessaging.SecondParticipantUserId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// Deletes an existing direct messaging entry based on the provided id.
        /// </summary>
        /// <param name="directMessagingId">The id of the target direct messaging entry</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{directMessagingId:long}")]
        [ProducesResponseType(typeof(DirectMessaging), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Delete(long directMessagingId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _directMessagingService.Delete(userId, directMessagingId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// Fetches all messages of a direct messaging entry based on the provided id.
        /// When a message id is also provided then it will return 300 messages prior to that message.
        /// </summary>
        /// <param name="directMessagingId">The id of the target direct messaging entry</param>
        /// <param name="lastMessageId">The message id from which prior messages will be fetched</param>
        /// <returns>An IActionResult entry</param>
        [HttpGet("{directMessagingId:long}/messages")]
        [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetMessages(long directMessagingId, [FromQuery] long? lastMessageId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _directMessagingService.GetMessages(userId, directMessagingId, lastMessageId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// Inserts a new message to a direct messaging entry.
        /// </summary>
        /// <param name="message">The message to be inserted.</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("{directMessagingId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PostMessage(long directMessagingId, [FromBody] Message message)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _directMessagingService.InsertMessage(userId, directMessagingId, message));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// Updates an existing message of a direct messaging entry.
        /// </summary>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut("{directMessagingId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PutMessage(long directMessagingId, [FromBody] Message message)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _directMessagingService.UpdateMessage(userId, directMessagingId, message));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        /// Deletes a message from a direct messaging entry.
        /// </summary>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{directMessagingId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult DeleteMessage(long directMessagingId, [FromBody] long messageId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _directMessagingService.DeleteMessage(userId, directMessagingId, messageId));

            return this.UnwrapMonad(monad);
        }
    }
}
