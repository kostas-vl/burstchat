using System;
using System.Threading.Tasks;
using BurstChat.Application.Models;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.ChannelsService;
using BurstChat.Application.Services.DirectMessagingService;
using BurstChat.Application.Services.PrivateGroupsService;
using BurstChat.Application.Services.ServersService;
using BurstChat.Application.Services.UserService;
using BurstChat.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal.Hubs.Chat;

[Authorize]
public partial class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IUserService _userService;
    private readonly IServersService _serverService;
    private readonly IPrivateGroupsService _privateGroupService;
    private readonly IChannelsService _channelsService;
    private readonly IDirectMessagingService _directMessagingService;

    public ChatHub(
        ILogger<ChatHub> logger,
        IUserService userService,
        IServersService serverService,
        IPrivateGroupsService privateGroupService,
        IChannelsService channelsService,
        IDirectMessagingService directMessagingService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _serverService = serverService ?? throw new ArgumentNullException(nameof(serverService));
        _privateGroupService =
            privateGroupService ?? throw new ArgumentNullException(nameof(privateGroupService));
        _channelsService =
            channelsService ?? throw new ArgumentNullException(nameof(channelsService));
        _directMessagingService =
            directMessagingService
            ?? throw new ArgumentNullException(nameof(directMessagingService));
    }

    public override Task OnConnectedAsync() =>
        Context
            .GetHttpContext()
            .GetUserId()
            .InspectAsync(async userId =>
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
            });

    public override Task OnDisconnectedAsync(Exception? e) =>
        _logger
            .Inspect(l => l.LogError(e?.Message))
            .And(_ => Context.GetHttpContext().GetUserId())
            .InspectAsync(async userId =>
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
            });

    public Task UpdateMyInfo() =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _userService.Get(userId))
            .InspectAsync(async user =>
            {
                await Clients.Groups(Context.ConnectionId).UserUpdated(user);
                await Clients.Caller.UserUpdated(user);
            })
            .InspectErrAsync(err => Clients.Caller.UserUpdated(err.Into()));

    public Task GetInvitations() =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _userService.GetInvitations(userId))
            .InspectAsync(invitations => Clients.Caller.Invitations(invitations))
            .InspectErrAsync(err => Clients.Caller.Invitations(err.Into()));

    public Task SendInvitation(int serverId, string username) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .AndAsync(userId =>
                _serverService
                    .InsertInvitation(userId, serverId, username)
                    .InspectAsync(inv => Clients.Groups(userId.ToString()).NewInvitation(inv))
            )
            .InspectErrAsync(err => Clients.Caller.NewInvitation(err.Into()));

    public Task UpdateInvitation(long id, bool accepted) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId =>
            {
                var data = new UpdateInvitation { InvitationId = id, Accepted = accepted };
                return _userService.UpdateInvitation(userId, data);
            })
            .InspectAsync(async inv =>
            {
                var signalGroup = ServerSignalName(inv.ServerId);
                if (inv.Accepted)
                    await Clients.Group(signalGroup).UpdatedInvitation(inv);
                await Clients.Caller.UpdatedInvitation(inv);
            })
            .InspectErrAsync(err => Clients.Caller.UpdatedInvitation(err.Into()));
}
