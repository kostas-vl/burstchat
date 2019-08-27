using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Shared.Schema.Users;
using BurstChat.Signal.Models;
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
        ///   Adds a new connection to a signalr group that is based on the id of a private BurstChat group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>A Task instance</returns>
        public async Task AddToPrivateGroupConnection(long groupId)
        {
            var monad = await _privateGroupMessagingService.GetPrivateGroupAsync(groupId);

            if (monad is Success<PrivateGroupMessage, Error>)
            {
                var signalGroup = groupId.ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
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
            var monad = await _privateGroupMessagingService.GetAllAsync(groupId);

            if (monad is Success<IEnumerable<Message>, Error> success)
            {
                var payload = new Payload<IEnumerable<Message>>
                {
                    SignalGroup = groupId.ToString(),
                    Content = success.Value
                };
                await Clients.Caller.AllPrivateGroupMessages(payload);
            }
            else if (monad is Failure<IEnumerable<Message>, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = groupId.ToString(),
                    Content = failure.Value
                };
                await Clients.Caller.AllPrivateGroupMessages(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = groupId.ToString(),
                    Content = SystemErrors.Exception()
                };
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
            var monad = await _privateGroupMessagingService.PostAsync(groupId, message);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = groupId.ToString(),
                    Content = message
                };
                await Clients.Group(payload.SignalGroup).PrivateGroupMessageReceived(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = groupId.ToString(),
                    Content = failure.Value
                };
                await Clients.Caller.PrivateGroupMessageReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = groupId.ToString(),
                    Content = SystemErrors.Exception()
                };
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
            var monad = await _privateGroupMessagingService.PostAsync(groupId, message);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = groupId.ToString(),
                    Content = message
                };
                await Clients.Groups(payload.SignalGroup).PrivateGroupMessageEdited(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = groupId.ToString(),
                    Content = failure.Value
                };
                await Clients.Caller.PrivateGroupMessageEdited(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = groupId.ToString(),
                    Content = SystemErrors.Exception()
                };
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
            var monad = await _privateGroupMessagingService.DeleteAsync(groupId, message);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = groupId.ToString(),
                    Content = message
                };
                await Clients.Groups(payload.SignalGroup).PrivateGroupMessageDeleted(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = groupId.ToString(),
                    Content = failure.Value
                };
                await Clients.Caller.PrivateGroupMessageDeleted(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = groupId.ToString(),
                    Content = SystemErrors.Exception()
                };
                await Clients.Caller.PrivateGroupMessageDeleted(payload);
            }
        }

        /// <summary>
        ///   Adds a new connection to a signalr group based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>A task instance</returns>
        public async Task AddToChannelConnection(int channelId)
        {
            var monad = await _channelsService.GetChannelAsync(channelId);

            if (monad is Success<Channel, Error>)
            {
                var groupName = channelId.ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
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
            {
                var payload = new Payload<IEnumerable<Message>>
                {
                    SignalGroup = channelId.ToString(),
                    Content = success.Value
                };
                await Clients.Caller.AllChannelMessagesReceived(payload);
            }
            else if (monad is Failure<IEnumerable<Message>, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = channelId.ToString(),
                    Content = failure.Value
                };
                await Clients.Caller.AllChannelMessagesReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = channelId.ToString(),
                    Content = SystemErrors.Exception()
                };
                await Clients.Caller.AllChannelMessagesReceived(payload);
            }
        }

        /// <summary>
        ///   Informs the users of a channel about a new message that was posted.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task instance</returns>
        public async Task PostChannelMessage(int channelId, Message message)
        {
            var groupName = channelId.ToString();
            var monad = await _channelsService.PostAsync(channelId, message);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = channelId.ToString(),
                    Content = message
                };
                await Clients.Groups(payload.SignalGroup).ChannelMessageReceived(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = channelId.ToString(),
                    Content = failure.Value
                };
                await Clients.Caller.ChannelMessageReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = channelId.ToString(),
                    Content = SystemErrors.Exception()
                };
                await Clients.Caller.ChannelMessageReceived(payload);
            }
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
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = channelId.ToString(),
                    Content = message
                };
                await Clients.Groups(payload.SignalGroup).ChannelMessageEdited(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = channelId.ToString(),
                    Content = failure.Value
                };
                await Clients.Caller.ChannelMessageEdited(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = channelId.ToString(),
                    Content = SystemErrors.Exception()
                };
                await Clients.Caller.ChannelMessageEdited(payload);
            }
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
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = channelId.ToString(),
                    Content = message
                };
                await Clients.Groups(payload.SignalGroup).ChannelMessageDeleted(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = channelId.ToString(),
                    Content = failure.Value
                };
                await Clients.Caller.ChannelMessageDeleted(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = channelId.ToString(),
                    Content = SystemErrors.Exception()
                };
                await Clients.Caller.ChannelMessageDeleted(payload);
            }
        }
    }
}
