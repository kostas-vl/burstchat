using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Shared.Schema.Users;
using BurstChat.Signal.Models;
using BurstChat.Signal.Services.ChannelsService;
using BurstChat.Signal.Services.InvitationsService;
using BurstChat.Signal.Services.PrivateGroupMessaging;
using BurstChat.Signal.Services.ServerService;
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
        private readonly IInvitationsService _invitationsService;
        private readonly IServerService _serverService;
        private readonly IPrivateGroupMessagingService _privateGroupMessagingService;
        private readonly IChannelsService _channelsService;

        /// <summary>
        ///   Executes any necessary start up code for the hub.
        /// </summary>
        public ChatHub(
            ILogger<ChatHub> logger,
            IInvitationsService invitationsService,
            IServerService serverService,
            IPrivateGroupMessagingService privateGroupMessagingService,
            IChannelsService channelsService
        )
        {
            _logger = logger;
            _invitationsService = invitationsService;
            _serverService = serverService;
            _privateGroupMessagingService = privateGroupMessagingService;
            _channelsService = channelsService;
        }

        /// <summary>
        ///     Constructs the appropriate signal server name of all users of a server.
        /// </summary>
        /// <param name="id">The id of the server</param>
        /// <returns>The string signal server name</returns>
        private string ServerSignalName(int id) => $"server:{id}";

        /// <summary>
        ///   Constructs the appropriate signal group name for a private group chat.
        /// </summary>
        /// <param name="id">The id of the private group</param>
        /// <returns>The string signal group name</returns>
        private string PrivateGroupSignalName(long id) => $"privateGroup:{id}";

        /// <summary>
        ///   Constructs the appropriate signal group name for a channel chat.
        /// </summary>
        /// <param name="id">The id of the channel</param>
        /// <returns>The string signal channel name</param>
        private string ChannelSignalName(int id) => $"channel:{id}";

        /// <summary>
        ///     Adds a new server and informs the caller of the results.
        /// </summary>
        /// <param name="server">The server instance to be added</param>
        /// <returns>A Task instance</returns>
        public async Task AddServer(Server server)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _serverService.PostAsync(httpContext, server);

            if (monad is Success<Server, Error> success)
                await Clients.Caller.AddedServer(success.Value);
            else if (monad is Failure<Server, Error> failure)
                await Clients.Caller.AddedServer(failure.Value);
            else
                await Clients.Caller.AddedServer(SystemErrors.Exception());
        }

        /// <summary>
        ///     Adds a new connection to a signalr group that is based on the id of a BurstChat server.
        /// </summary>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>A Task instance</returns>
        public async Task AddToServer(int serverId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _serverService.GetAsync(httpContext, serverId);

            if (monad is Success<Server, Error>)
            {
                var signalGroup = ServerSignalName(serverId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
            }
        }

        /// <summary>
        ///     Returns all invitations sent to a user.
        /// </summary>
        /// <returns>A Task instance</returns>
        public async Task GetInvitations()
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _invitationsService.GetAllAsync(httpContext);

            if (monad is Success<IEnumerable<Invitation>, Error> success)
                await Clients.Caller.Invitations(success.Value);
            else if (monad is Failure<IEnumerable<Invitation>, Error> failure)
                await Clients.Caller.Invitations(failure.Value);
            else
                await Clients.Caller.Invitations(SystemErrors.Exception());
        }

        /// <summary>
        ///     Sends to a user a new server invitation.
        /// </summary>
        /// <param name="server">The id of the server the invitation is from</param>
        /// <param name="userId">The id of the user the invitation will be sent</param>
        /// <returns>A Task instance</returns>
        public async Task SendInvitation(int serverId, long userId)
        {
            var httpContext = Context.GetHttpContext();
            var requestingUserId = httpContext.GetUserId().ToString();
            var monad = await _invitationsService.InsertAsync(httpContext, serverId, userId);

            if (monad is Success<Invitation, Error> success)
                await Clients.User(requestingUserId).NewInvitation(success.Value);
            else if (monad is Failure<Invitation, Error> failure)
                await Clients.Caller.NewInvitation(failure.Value);
            else
                await Clients.Caller.NewInvitation(SystemErrors.Exception());
        }

        /// <summary>
        ///     Updates an invitation's state and informs the appropriate users.
        /// </summary>
        /// <param name="invitation">The invitation to be updated</param>
        /// <returns>A Task instance</returns>
        public async Task UpdateInvitation(Invitation invitation)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _invitationsService.UpdateAsync(httpContext, invitation);

            if (monad is Success<Invitation, Error> success)
            {
                var value = success.Value;
                var signalGroup = ServerSignalName(value.ServerId);

                if (value.Accepted)
                {
                    await Clients.Group(signalGroup).UpdatedInvitation(value);
                    await Clients.Caller.UpdatedInvitation(value);
                }
                else
                    await Clients.Caller.UpdatedInvitation(value);
            }
            else if (monad is Failure<Invitation, Error> failure)
                await Clients.Caller.UpdatedInvitation(failure.Value);
            else
                await Clients.Caller.UpdatedInvitation(SystemErrors.Exception());
        }

        /// <summary>
        ///   Adds a new connection to a signalr group that is based on the id of a private BurstChat group.
        /// </summary>
        /// <param name="groupId">The id of the target group</param>
        /// <returns>A Task instance</returns>
        public async Task AddToPrivateGroupConnection(long groupId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.GetPrivateGroupAsync(httpContext, groupId);

            if (monad is Success<PrivateGroupMessage, Error>)
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
                var payload = new Payload<IEnumerable<Message>>
                {
                    SignalGroup = signalGroup,
                    Content = success.Value
                };
                await Clients.Caller.AllPrivateGroupMessages(payload);
            }
            else if (monad is Failure<IEnumerable<Message>, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = failure.Value
                };
                await Clients.Caller.AllPrivateGroupMessages(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
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
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.PostAsync(httpContext, groupId, message);
            var signalGroup = PrivateGroupSignalName(groupId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = signalGroup,
                    Content = message
                };
                await Clients.Group(signalGroup).PrivateGroupMessageReceived(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = failure.Value
                };
                await Clients.Caller.PrivateGroupMessageReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
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
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.PostAsync(httpContext, groupId, message);
            var signalGroup = PrivateGroupSignalName(groupId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = signalGroup,
                    Content = message
                };
                await Clients.Groups(signalGroup).PrivateGroupMessageEdited(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = failure.Value
                };
                await Clients.Caller.PrivateGroupMessageEdited(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
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
            var httpContext = Context.GetHttpContext();
            var monad = await _privateGroupMessagingService.DeleteAsync(httpContext, groupId, message);
            var signalGroup = PrivateGroupSignalName(groupId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = signalGroup,
                    Content = message
                };
                await Clients.Groups(signalGroup).PrivateGroupMessageDeleted(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = failure.Value
                };
                await Clients.Caller.PrivateGroupMessageDeleted(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
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
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.GetChannelAsync(httpContext, channelId);

            if (monad is Success<Channel, Error>)
            {
                var signalGroup = ChannelSignalName(channelId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.SelfAddedToChannel();
            }
        }

        /// <summary>
        ///   Informs the caller of all messages posted to a channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>A task instance</returns>
        public async Task GetAllChannelMessages(int channelId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.GetAllAsync(httpContext, channelId);
            var signalGroup = ChannelSignalName(channelId);

            if (monad is Success<IEnumerable<Message>, Error> success)
            {
                var payload = new Payload<IEnumerable<Message>>
                {
                    SignalGroup = signalGroup,
                    Content = success.Value
                };
                await Clients.Caller.AllChannelMessagesReceived(payload);
            }
            else if (monad is Failure<IEnumerable<Message>, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = failure.Value
                };
                await Clients.Caller.AllChannelMessagesReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
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
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.PostAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = signalGroup,
                    Content = message
                };
                await Clients.Groups(signalGroup).ChannelMessageReceived(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = failure.Value
                };
                await Clients.Caller.ChannelMessageReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
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
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.PutAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = signalGroup,
                    Content = message
                };
                await Clients.Groups(signalGroup).ChannelMessageEdited(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = failure.Value
                };
                await Clients.Caller.ChannelMessageEdited(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
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
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.DeleteAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>
                {
                    SignalGroup = signalGroup,
                    Content = message
                };
                await Clients.Groups(signalGroup).ChannelMessageDeleted(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = failure.Value
                };
                await Clients.Caller.ChannelMessageDeleted(payload);
            }
            else
            {
                var payload = new Payload<Error>
                {
                    SignalGroup = signalGroup,
                    Content = SystemErrors.Exception()
                };
                await Clients.Caller.ChannelMessageDeleted(payload);
            }
        }
    }
}
