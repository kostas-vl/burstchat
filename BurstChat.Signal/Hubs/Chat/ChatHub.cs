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
        /// This method is an override of the virtual OnConnectedAsync, and creates a single user group of the
        /// authenticated user with his user id.
        /// </summary>
        /// <returns>A Task instance</returns>
        public override async Task OnConnectedAsync()
        {
            await Context
                .GetHttpContext()
                .GetUserId()
                .ExecuteAndContinueAsync(async userId => 
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
                });
        }

        /// <summary>
        /// This method is an override of the virtual OnDisconnectedAsync, and removes the single user group of the
        /// authenticated user.
        /// </summary>
        /// <param name="e">The incoming exception</param>
        /// <returns>A Task instance</returns>
        public override async Task OnDisconnectedAsync(Exception e)
        {
            _logger.LogException(e);

            await Context
                .GetHttpContext()
                .GetUserId()
                .ExecuteAndContinueAsync(async userId =>
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());                    
                });
        }

        /// <summary>
        ///     Returns all invitations sent to a user.
        /// </summary>
        /// <returns>A Task instance</returns>
        public async Task GetInvitations()
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _invitationsService.GetAllAsync(httpContext);

            switch (monad)
            {
                case Success<IEnumerable<Invitation>, Error> success:
                    await Clients.Caller.Invitations(success.Value);
                    break;

                case Failure<IEnumerable<Invitation>, Error> failure:
                    await Clients.Caller.Invitations(failure.Value);
                    break;

                default:
                    await Clients.Caller.Invitations(SystemErrors.Exception());
                    break;
            }
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

            switch (monad)
            {
                case Success<Invitation, Error> success:
                    var userId = success.Value.UserId.ToString();
                    await Clients.Groups(userId).NewInvitation(success.Value);
                    break;

                case Failure<Invitation, Error> failure:
                    await Clients.Caller.NewInvitation(failure.Value);  
                    break;
                
                default:
                    await Clients.Caller.NewInvitation(SystemErrors.Exception());
                    break;
            }
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
            var signalGroup = string.Empty;
            var invite = new Invitation();

            switch (monad)
            {
                case Success<Invitation, Error> success when success.Value.Accepted:
                    invite = success.Value;
                    signalGroup = ServerSignalName(invite.ServerId);
                    await Clients.Group(signalGroup).UpdatedInvitation(invite);
                    await Clients.Caller.UpdatedInvitation(invite);
                    break;

                case Success<Invitation, Error> success when !success.Value.Accepted:
                    invite = success.Value;
                    signalGroup = ServerSignalName(invite.ServerId);
                    await Clients.Caller.UpdatedInvitation(invite);
                    break;

                case Failure<Invitation, Error> failure:
                    await Clients.Caller.UpdatedInvitation(failure.Value);
                    break;
                
                default:
                    await Clients.Caller.UpdatedInvitation(SystemErrors.Exception());
                    break;
            }
        }
    }
}
