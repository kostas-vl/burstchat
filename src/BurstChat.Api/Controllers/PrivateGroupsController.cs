using System;
using System.Collections.Generic;
using BurstChat.Api.ActionResults;
using BurstChat.Api.Extensions;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.PrivateGroupsService;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    /// This class represents an ASPNET Core controller that exposes endpoints for interacting with
    /// data of private groups.
    /// </summary>
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("/api/groups")]
    public class PrivateGroupsController : ControllerBase
    {
        private readonly IPrivateGroupsService _privateGroupMessagingService;

        /// <summary>
        /// Executes any neccessary start up code for the controller.
        /// 
        /// Exceptions:
        ///     ArgumentNullException: When any parameter is null.
        /// </summary>
        public PrivateGroupsController(IPrivateGroupsService privateGroupsService)
        {
            _privateGroupMessagingService = privateGroupsService
                ?? throw new ArgumentNullException(nameof(privateGroupsService));
        }

        /// <summary>
        /// Fetches all available information about a group and its users.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpGet("{groupId:long}")]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<PrivateGroup, Error> Get(long groupId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.Get(userId, groupId));

        /// <summary>
        /// Creates a new messaging group based on the name provided.
        /// </summary>
        /// <param name="groupName">The name of the new group</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<PrivateGroup, Error> Post([FromBody] string groupName) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.Insert(userId, groupName));

        /// <summary>
        /// Creates a new messaging group with specific users as members,
        /// based on the paramters provided.
        /// </summary>
        /// <param name="groupName">The name of the new group</param>
        /// <param name="userIds">The id of the users to be members</param>
        /// <returns>A MonadActionResult instance</returns>
        [HttpPost("{groupName}")]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<PrivateGroup, Error> Post(string groupName, [FromBody] IEnumerable<long> userIds) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.Insert(userId, groupName))
                       .Bind(privateGroup => HttpContext.GetUserId()
                                                        .Bind(userId => _privateGroupMessagingService.InsertUsers(userId, privateGroup.Id, userIds)));

        /// <summary>
        /// Removes an existing messaging group based on the id provided.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpDelete("{groupId:long}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Unit, Error> Delete(long groupId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.Delete(userId, groupId));

        /// <summary>
        /// Adds a new user to a messaging group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userId">The id of the user to be added</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpPost("{groupId:long}/user")]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<PrivateGroup, Error> PostUser(long groupId, [FromBody] long newUserId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.InsertUser(userId, groupId, newUserId));

        /// <summary>
        /// Removes a user from a messaging group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="userId">The id of the user to be deleted</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpDelete("{groupId:long}/user")]
        [ProducesResponseType(typeof(PrivateGroup), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<PrivateGroup, Error> DeleteUser(long groupId, [FromBody] long targetUserId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.DeleteUser(userId, groupId, targetUserId));

        /// <summary>
        /// Fetches all available messages for a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</summary>
        /// <returns>An MonadActionResult instance</returns>
        [HttpGet("{groupId:long}/messages")]
        [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<IEnumerable<Message>, Error> GetMessages(long groupId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.GetMessages(userId, groupId));

        /// <summary>
        /// Adds a new message to a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="message">The message to be added</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpPost("{groupId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Message, Error> PostMessage(long groupId, [FromBody] Message message) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.InsertMessage(userId, groupId, message));

        /// <summary>
        /// Updates an existing message of a group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpPut("{groupId:long}/messages")]
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Message, Error> PutMessage(long groupId, [FromBody] Message message) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.UpdateMessage(userId, groupId, message));

        /// <summary>
        /// Removes an existing message from a group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        /// <returns>An MonadActionResult instance</returns>
        [HttpDelete("{groupId:long}/messages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        public MonadActionResult<Unit, Error> DeleteMessage(long groupId, [FromBody] long messageId) =>
            HttpContext.GetUserId()
                       .Bind(userId => _privateGroupMessagingService.DeleteMessage(userId, groupId, messageId));
    }
}
