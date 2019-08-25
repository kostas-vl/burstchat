using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Signal.Services.ChannelsService;
using BurstChat.Signal.Services.PrivateGroupMessaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal.Hubs.Chat
{
    /// <summary>
    ///   This class is a SignalR hub that is responsible for message of the delivered to the users of the chat.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IPrivateGroupMessagingService _privateGroupMessagingService;
        private readonly IChannelsService _channelsService;

        /// <summary>
        ///   Executes any necessary start up code for the hub.
        /// </summary>
        public ChatHub(
            ILogger<ChatHub> logger,
            IPrivateGroupMessagingService privateGroupMessagingService,
            IChannelsService channelsService
        )
        {
            _logger = logger;
            _privateGroupMessagingService = privateGroupMessagingService;
            _channelsService = channelsService;
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

        /// <summary>
        ///   Informs the caller of all messages posted to a channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>A task instance</returns>
        public async Task GetAllChannelMessages(int channelId)
        {
            var monad = await _channelsService.GetAllAsync(channelId);

            if (monad is Success<IEnumerable<Message>, Error> success)
                await Clients.Caller.AllChannelMessagesReceived(success.Value);
            else if (monad is Failure<IEnumerable<Message>, Error> failure)
                await Clients.Caller.AllChannelMessagesReceived(failure.Value);
            else
                await Clients.Caller.AllChannelMessagesReceived(SystemErrors.Exception());
        }

        /// <summary>
        ///   Informs the users of a channel about a new message that was posted.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task instance</returns>
        public async Task PostChannelMessage(int channelId, Message message)
        {
            var monad = await _channelsService.PostAsync(channelId, message);

            if (monad is Success<Unit, Error> success)
                await Clients.All.ChannelMessageReceived(message);
            else if (monad is Failure<Unit, Error> failure)
                await Clients.Caller.ChannelMessageReceived(failure.Value);
            else 
                await Clients.Caller.ChannelMessageReceived(SystemErrors.Exception());
        }

        /// <summary>
        ///   Informs the users of a channel that a message was edited.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task instance</returns>
        public async Task PutChannelMessage(int channelId, Message message)
        {
            var monad = await _channelsService.PutAsync(channelId, message);

            if (monad is Success<Unit, Error> success)
                await Clients.All.ChannelMessageEdited(message);
            else if (monad is Failure<Unit, Error> failure)
                await Clients.Caller.ChannelMessageEdited(failure.Value);
            else
                await Clients.Caller.ChannelMessageEdited(SystemErrors.Exception());
        }

        /// <summary>
        ///   Informs the users of a channel that a message was deleted.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task instance</returns>
        public async Task DeleteChannelMessage(int channelId, Message message)
        {
            var monad = await _channelsService.DeleteAsync(channelId, message);

            if (monad is Success<Unit, Error> success)
                await Clients.All.ChannelMessageDeleted(message);
            else if (monad is Failure<Unit, Error> failure)
                await Clients.Caller.ChannelMessageDeleted(failure.Value);
            else 
                await Clients.Caller.ChannelMessageDeleted(SystemErrors.Exception());
        }
    }
}
