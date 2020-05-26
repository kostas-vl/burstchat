using System.Collections.Generic;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Services.PrivateGroupsService;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    ///   This class represents an ASPNET Core controller that exposes endpoints for interacting with
    ///   data of private groups.
    /// </summary>
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("/api/groups")]
    public class PrivateGroupsController : ControllerBase
    {
        private readonly IPrivateGroupsService _privateGroupMessagingService;

        /// <summary>
        ///   Executes any neccessary start up code for the controller.
        /// </summary>
        public PrivateGroupsController(IPrivateGroupsService privateGroupsService)
        {
            _privateGroupMessagingService = privateGroupsService;
        }

        /// <summary>
        ///   Fetches all available information about a group and its users.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{groupId:long}")]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Get(long groupId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.Get(userId, groupId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Creates a new messaging group based on the name provided.
        /// </summary>
        /// <param name="groupName">The name of the new group</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Post([FromBody] string groupName)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.Insert(userId, groupName));

            return this.UnwrapMonad(monad);
        }

        [HttpPost("{groupName}")]
        public IActionResult Post(string groupName, [FromBody] IEnumerable<long> userIds)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.Insert(userId, groupName))
                .Bind(privateGroup => HttpContext.GetUserId()
                                                 .Bind(userId => _privateGroupMessagingService.InsertUsers(userId, privateGroup.Id, userIds)));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Removes an existing messaging group based on the id provided.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{groupId:long}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult Delete(long groupId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.Delete(userId, groupId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Adds a new user to a messaging group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userId">The id of the user to be added</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("{groupId:long}/user")]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PostUser(long groupId, [FromBody] long newUserId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.InsertUser(userId, groupId, newUserId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Removes a user from a messaging group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userId">The id of the user to be deleted</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{groupId:long}/user")]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult DeleteUser(long groupId, [FromBody] long targetUserId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.DeleteUser(userId, groupId, targetUserId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Fetches all available messages for a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</summary>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{groupId:long}/messages")]
        [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult GetMessages(long groupId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.GetMessages(userId, groupId));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Adds a new message to a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="message">The message to be added</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("{groupId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PostMessage(long groupId, [FromBody] Message message)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.InsertMessage(userId, groupId, message));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Updates an existing message of a group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut("{groupId:long}/messages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult PutMessage(long groupId, [FromBody] Message message)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.UpdateMessage(userId, groupId, message));

            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Removes an existing message from a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{groupId:long}/messages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public IActionResult DeleteMessage(long groupId, [FromBody] long messageId)
        {
            var monad = HttpContext
                .GetUserId()
                .Bind(userId => _privateGroupMessagingService.DeleteMessage(userId, groupId, messageId));

            return this.UnwrapMonad(monad);
        }
    }
}
