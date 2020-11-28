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
        /// 
        /// Exceptions:
        ///     ArgumentNullException: When any parameter is null.
        /// </summary>
        public DirectMessagingController(IDirectMessagingService directMessagingService)
        {
            _directMessagingService = directMessagingService 
                ?? throw new ArgumentNullException(nameof(directMessagingService));
        }

        /// <summary>
        /// Fetches all available information about the direct messages of a user.
        /// </summary>
        /// <param name="directMessagingId">The id of the direct messages</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("{directMessagingId:long}")]
        [ProducesResponseType(typeof(DirectMessaging), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<DirectMessaging, Error> Get(long directMessagingId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _directMessagingService.Get(userId, directMessagingId));

        /// <summary>
        /// Fetches all available information about the direct messages between two users.
        /// </summary>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the second participant</param>
        /// <returns>A MonadActionResult instance</returns>
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

        /// <summary>
        /// Fetches all users that the authenticated user has sent a direct messages.
        /// </summary>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("users")]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<User?>, Error> GetUsers() =>
            HttpContext.GetUserId()
                       .Bind(_directMessagingService.GetUsers);

        /// <summary>
        /// Creates a new direct messaging association between 2 users.
        /// </summary>
        /// <param name="directMessaging">The direct messaging instance to be created</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DirectMessaging), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<DirectMessaging, Error> Post([FromBody] DirectMessaging directMessaging) =>
            HttpContext.GetUserId()
                       .Bind(userId => _directMessagingService.Insert(userId,
                                                                      directMessaging.FirstParticipantUserId,
                                                                      directMessaging.SecondParticipantUserId));

        /// <summary>
        /// Deletes an existing direct messaging entry based on the provided id.
        /// </summary>
        /// <param name="directMessagingId">The id of the target direct messaging entry</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpDelete("{directMessagingId:long}")]
        [ProducesResponseType(typeof(DirectMessaging), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<DirectMessaging, Error> Delete(long directMessagingId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _directMessagingService.Delete(userId, directMessagingId));

        /// <summary>
        /// Fetches all messages of a direct messaging entry based on the provided id.
        /// When a message id is also provided then it will return 300 messages prior to that message.
        /// </summary>
        /// <param name="directMessagingId">The id of the target direct messaging entry</param>
        /// <param name="lastMessageId">The message id from which prior messages will be fetched</param>
        /// <returns>A MonadActionResult entry</param>
        [HttpGet("{directMessagingId:long}/messages")]
        [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<Message>, Error> GetMessages(long directMessagingId, [FromQuery] long? lastMessageId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _directMessagingService.GetMessages(userId, directMessagingId, lastMessageId));

        /// <summary>
        /// Inserts a new message to a direct messaging entry.
        /// </summary>
        /// <param name="message">The message to be inserted.</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost("{directMessagingId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Message, Error> PostMessage(long directMessagingId, [FromBody] Message message) =>
            HttpContext.GetUserId()
                       .Bind(userId => _directMessagingService.InsertMessage(userId, directMessagingId, message));

        /// <summary>
        /// Updates an existing message of a direct messaging entry.
        /// </summary>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPut("{directMessagingId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Message, Error> PutMessage(long directMessagingId, [FromBody] Message message) =>
            HttpContext.GetUserId()
                       .Bind(userId => _directMessagingService.UpdateMessage(userId, directMessagingId, message));

        /// <summary>
        /// Deletes a message from a direct messaging entry.
        /// </summary>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpDelete("{directMessagingId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Message, Error> DeleteMessage(long directMessagingId, [FromBody] long messageId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _directMessagingService.DeleteMessage(userId, directMessagingId, messageId));
    }
}
