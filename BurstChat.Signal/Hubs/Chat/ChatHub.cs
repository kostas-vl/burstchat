using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Signal.Services.PrivateGroupMessaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal.Hubs.Chat
{
    /// <summary>
    ///   This class is a SignalR hub that is responsible for message of the delivered to the users of the chat.
    /// </summary>
    public class ChatHub : Hub<IChatClient>
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IPrivateGroupMessagingService _privateGroupMessagingService;

        /// <summary>
        ///   Executes any necessary start up code for the hub.
        /// </summary>
        public ChatHub(
            ILogger<ChatHub> logger,
            IPrivateGroupMessagingService privateGroupMessagingService
        )
        {
            _logger = logger;
            _privateGroupMessagingService = privateGroupMessagingService;
        }

        /// <summary>
        ///   Sends a new message to the caller that contains either all the messages posted to a
        ///   group or an error explaining why the operation wasn't completed properly.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>A Task instance</returns>
        public async Task GetAllPrivateGroupMessages(long groupId)
        {
            var monad = await _privateGroupMessagingService.GetAllAsync(groupId);

            if (monad is Success<IEnumerable<Message>, Error> success)
                await Clients.Caller.AllPrivateGroupMessages(success.Value);
            else if (monad is Failure<IEnumerable<Message>, Error> failure)
                await Clients.Caller.AllPrivateGroupMessages(failure.Value);
            else
                await Clients.Caller.AllPrivateGroupMessages(SystemErrors.Exception());
        }

        /// <summary>
        /// Sends a new message to all users from the parameters provided.
        /// </summary>
        /// <param name="message">The message to be sent to connected users</param>
        /// <returns>A task instance</returns>
        public async Task PostPrivateGroupMessage(long groupId, Message message)
        {
            var monad = await _privateGroupMessagingService.PostAsync(groupId, message);

            if (monad is Success<Unit, Error> success)
                await Clients.All.PrivateGroupMessageReceived(message);
            else if (monad is Failure<Unit, Error> failure)
                await Clients.Caller.PrivateGroupMessageReceived(message);
            else
                await Clients.Caller.PrivateGroupMessageReceived(message);
        }

        /// <summary>
        ///   Sends an edited message to all users based on the parameters provided.
        /// </summary>
        /// <param name="message">The message that was edited and will be sent to connected users</param>
        /// <returns>A task instance</returns>
        public async Task PutPrivateGroupMessage(long groupId, Message message)
        {
            var monad = await _privateGroupMessagingService.PostAsync(groupId, message);

            if (monad is Success<Unit, Error> success)
                await Clients.All.PrivateGroupMessageEdited(message);
            else if (monad is Failure<Unit, Error> failure)
                await Clients.Caller.PrivateGroupMessageEdited(failure.Value);
            else
                await Clients.Caller.PrivateGroupMessageEdited(SystemErrors.Exception());
        }

        /// <summary>
        ///   Informs all users that a message of a group was deleted based on the provided parameters.
        /// </summary>
        /// <param name="message">The message to be deleted and sent to connected users</param>
        /// <returns>A task instance</returns>
        public async Task DeletePrivateGroupMessage(long groupId, Message message)
        {
            var monad = await _privateGroupMessagingService.DeleteAsync(groupId, message);

            if (monad is Success<Unit, Error> success)
                await Clients.All.PrivateGroupMessageDeleted(message);
            else if (monad is Failure<Unit, Error> failure)
                await Clients.Caller.PrivateGroupMessageDeleted(failure.Value);
            else
                await Clients.Caller.PrivateGroupMessageDeleted(SystemErrors.Exception());
        }
    }
}
