using System;
using BurstChat.Api.Extensions;
using BurstChat.Api.Services.PrivateGroupMessaging;
using BurstChat.Shared.Schema.Chat;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    ///   This class represents an ASPNET Core controller that exposes endpoints for interacting with
    ///   data of private groups.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("/api/groups")]
    public class PrivateGroupsController : ControllerBase
    {
        private readonly IPrivateGroupMessagingService _privateGroupMessagingService;

        /// <summary>
        ///   Executes any neccessary start up code for the controller.
        /// </summary>
        public PrivateGroupsController(IPrivateGroupMessagingService privateGroupMessagingService)
        {
            _privateGroupMessagingService = privateGroupMessagingService;
        }

        /// <summary>
        ///   Fetches all available information about a group and its users.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{groupId:long}")]
        public IActionResult Get(long groupId)
        {
            var monad = _privateGroupMessagingService.Get(groupId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Creates a new messaging group based on the name provided.
        /// </summary>
        /// <param name="groupName">The name of the new group</param>
        /// <returns>An IActionResult instance</returns>
        public IActionResult Post([FromBody] string groupName)
        {
            var monad = _privateGroupMessagingService.Insert(groupName);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Removes an existing messaging group based on the id provided.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{groupId:long}")]
        public IActionResult Delete(long groupId)
        {
            var monad = _privateGroupMessagingService.Delete(groupId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Adds a new user to a messaging group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userId">The id of the user to be added</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("{groupId:long}/user")]
        public IActionResult PostUser(long groupId, [FromBody] long userId)
        {
            var monad = _privateGroupMessagingService.InsertUser(groupId, userId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Removes a user from a messaging group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userId">The id of the user to be deleted</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{groupId:long}/user")]
        public IActionResult DeleteUser(long groupId, [FromBody] long userId)
        {
            var monad = _privateGroupMessagingService.DeleteUser(groupId, userId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Fetches all available messages for a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</summary>
        /// <returns>An IActionResult instance</returns>
        [HttpGet("{groupId:long}/messages")]
        public IActionResult GetMessages(long groupId)
        {
            var monad = _privateGroupMessagingService.GetMessages(groupId);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Adds a new message to a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="message">The message to be added</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPost("{groupId:long}/messages")]
        public IActionResult PostMessage(long groupId, [FromBody] Message message)
        {
            var monad = _privateGroupMessagingService.InsertMessage(groupId, message);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Updates an existing message of a group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>An IActionResult instance</returns>
        [HttpPut("{groupId:long}/messages")]
        public IActionResult PutMessage(long groupId, [FromBody] Message message)
        {
            var monad = _privateGroupMessagingService.UpdateMessage(groupId, message);
            return this.UnwrapMonad(monad);
        }

        /// <summary>
        ///   Removes an existing message from a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An IActionResult instance</returns>
        [HttpDelete("{groupId:long}/messages")]
        public IActionResult DeleteMessage(long groupId, [FromBody] long messageId)
        {
            var monad = _privateGroupMessagingService.DeleteMessage(groupId, messageId);
            return this.UnwrapMonad(monad);
        }
    }
}