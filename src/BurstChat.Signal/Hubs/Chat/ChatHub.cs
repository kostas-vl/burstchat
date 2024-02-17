using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Models;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Services.ChannelsService;
using BurstChat.Signal.Services.DirectMessagingService;
using BurstChat.Signal.Services.UserService;
using BurstChat.Signal.Services.PrivateGroupMessaging;
using BurstChat.Signal.Services.ServerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal.Hubs.Chat;

[Authorize]
public partial class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IUserService _userService;
    private readonly IServerService _serverService;
    private readonly IPrivateGroupMessagingService _privateGroupMessagingService;
    private readonly IChannelsService _channelsService;
    private readonly IDirectMessagingService _directMessagingService;

    public ChatHub(
        ILogger<ChatHub> logger,
        IUserService userService,
        IServerService serverService,
        IPrivateGroupMessagingService privateGroupMessagingService,
        IChannelsService channelsService,
        IDirectMessagingService directMessagingService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _serverService = serverService ?? throw new ArgumentNullException(nameof(serverService));
        _privateGroupMessagingService = privateGroupMessagingService ?? throw new ArgumentNullException(nameof(privateGroupMessagingService));
        _channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
        _directMessagingService = directMessagingService ?? throw new ArgumentNullException(nameof(directMessagingService));
    }

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

    public override async Task OnDisconnectedAsync(Exception? e)
    {
        _logger.LogError(e?.Message);

        await Context
            .GetHttpContext()
            .GetUserId()
            .ExecuteAndContinueAsync(async userId =>
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
            });
    }

    public async Task UpdateMyInfo()
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _userService.GetAsync(httpContext);

        switch (monad)
        {
            case Success<User, Error> success:
                await Clients.Groups(Context.ConnectionId).UserUpdated(success.Value);
                await Clients.Caller.UserUpdated(success.Value);
                break;

            case Failure<User, Error> failure:
                await Clients.Caller.UserUpdated(failure.Value);
                break;

            default:
                await Clients.Caller.UserUpdated(SystemErrors.Exception());
                break;
        }
    }

    public async Task GetInvitations()
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _userService.GetAllInvitationsAsync(httpContext);

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

    public async Task SendInvitation(int serverId, string username)
    {
        var httpContext = Context.GetHttpContext();
        var requestingUserId = httpContext.GetUserId().ToString();
        var monad = await _userService.InsertInvitationAsync(httpContext, serverId, username);

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

    public async Task UpdateInvitation(long id, bool accepted)
    {
        var httpContext = Context.GetHttpContext();
        var data = new UpdateInvitation
        {
            InvitationId = id,
            Accepted = accepted
        };
        var monad = await _userService.UpdateInvitationAsync(httpContext, data);
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
