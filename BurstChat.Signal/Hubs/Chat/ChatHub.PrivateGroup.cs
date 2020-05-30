using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Models;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat
{
    public partial class ChatHub
    {
        /// <summary>
        ///   Constructs the appropriate signal group name for a private group chat.
        /// </summary>
        /// <param name="id">The id of the private group</param>
        /// <returns>The string signal group name</returns>
        private string PrivateGroupSignalName(long id) => $"privateGroup:{id}";

        /// <summary>
        ///   Adds a new connection to a signalr group that is based on the id of a private BurstChat group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>A Task instance</returns>
        public async Task AddToPrivateGroupConnection(long groupId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.GetPrivateGroupAsync(httpContext, groupId);

            if (monad is Success<PrivateGroup, Error>)
            {
                var signalGroup = PrivateGroupSignalName(groupId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.SelfAddedToPrivateGroup();
            }
        }

        /// <summary>
        ///   Sends a new message to the caller that contains either all the messages posted to a
        ///   group or an error explaining why the operation wasn't completed properly.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>A task instance</returns>
        public async Task GetAllPrivateGroupMessages(long groupId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.GetAllAsync(httpContext, groupId);
            var signalGroup = PrivateGroupSignalName(groupId);

            if (monad is Success<IEnumerable<Message>, Error> success)
            {
                var payload = new Payload<IEnumerable<Message>>(signalGroup, success.Value);
                await Clients.Caller.AllPrivateGroupMessages(payload);
            }
            else if (monad is Failure<IEnumerable<Message>, Error> failure)
            {
                var payload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.AllPrivateGroupMessages(payload);
            }
            else
            {
                var payload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.AllPrivateGroupMessages(payload);
            }
        }

        /// <summary>
        ///   Sends a new message to all users from the parameters provided.
        /// </summary>
        /// <param name="message">The message to be sent to connected users</param>
        /// <returns>A task instance</returns>
        public async Task PostPrivateGroupMessage(long groupId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.PostAsync(httpContext, groupId, message);
            var signalGroup = PrivateGroupSignalName(groupId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Group(signalGroup).PrivateGroupMessageReceived(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.PrivateGroupMessageReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.PrivateGroupMessageReceived(payload);
            }
        }

        /// <summary>
        ///   Sends an edited message to all users based on the parameters provided.
        /// </summary>
        /// <param name="message">The message that was edited and will be sent to connected users</param>
        /// <returns>A task instance</returns>
        public async Task PutPrivateGroupMessage(long groupId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.PostAsync(httpContext, groupId, message);
            var signalGroup = PrivateGroupSignalName(groupId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).PrivateGroupMessageEdited(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.PrivateGroupMessageEdited(payload);
            }
            else
            {
                var payload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.PrivateGroupMessageEdited(payload);
            }
        }

        /// <summary>
        ///   Informs all users that a message of a group was deleted based on the provided parameters.
        /// </summary>
        /// <param name="message">The message to be deleted and sent to connected users</param>
        /// <returns>A task instance</returns>
        public async Task DeletePrivateGroupMessage(long groupId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.DeleteAsync(httpContext, groupId, message);
            var signalGroup = PrivateGroupSignalName(groupId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).PrivateGroupMessageDeleted(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.PrivateGroupMessageDeleted(payload);
            }
            else
            {
                var payload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.PrivateGroupMessageDeleted(payload);
            }
        }
    }
}
