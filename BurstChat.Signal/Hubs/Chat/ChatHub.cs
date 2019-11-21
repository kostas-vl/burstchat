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
using BurstChat.Signal.Services.DirectMessagingService;
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
    public partial class ChatHub : Hub<IChatClient>
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IInvitationsService _invitationsService;
        private readonly IServerService _serverService;
        private readonly IPrivateGroupMessagingService _privateGroupMessagingService;
        private readonly IChannelsService _channelsService;
        private readonly IDirectMessagingService _directMessagingService;

        /// <summary>
        ///   Executes any necessary start up code for the hub.
        /// </summary>
        public ChatHub(
            ILogger<ChatHub> logger,
            IInvitationsService invitationsService,
            IServerService serverService,
            IPrivateGroupMessagingService privateGroupMessagingService,
            IChannelsService channelsService,
            IDirectMessagingService directMessagingService
        )
        {
            _logger = logger;
            _invitationsService = invitationsService;
            _serverService = serverService;
            _privateGroupMessagingService = privateGroupMessagingService;
            _channelsService = channelsService;
            _directMessagingService = directMessagingService;
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
        /// <param name="username">The name of the user the invitation will be sent</param>
        /// <returns>A Task instance</returns>
        public async Task SendInvitation(int serverId, string username)
        {
            var httpContext = Context.GetHttpContext();
            var requestingUserId = httpContext.GetUserId().ToString();
            var monad = await _invitationsService.InsertAsync(httpContext, serverId, username);

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
    }
}
