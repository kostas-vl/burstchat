using System.Threading.Tasks;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Servers;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat;

public partial class ChatHub
{
    private string ServerSignalName(int id) => $"server:{id}";

    public Task AddServer(Server server) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _serverService.Insert(userId, server))
            .InspectAsync(server => Clients.Caller.AddedServer(server))
            .InspectErrAsync(err => Clients.Caller.AddedServer(err));

    public Task AddToServer(int serverId) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _serverService.Get(userId, serverId))
            .InspectAsync(async server =>
            {
                var signalGroup = ServerSignalName(serverId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
            });

    public Task UpdateServerInfo(Server server) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _serverService.Update(userId, server))
            .InspectAsync(async server =>
            {
                var signalGroup = ServerSignalName(server.Id);
                await Clients.Group(signalGroup).UpdatedServer(server);
            })
            .InspectErrAsync(err => Clients.Caller.UpdatedServer(err));

    public Task DeleteSubscription(int serverId, Subscription subscription) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _serverService.DeleteSubscription(userId, serverId, subscription))
            .InspectAsync(async sub =>
            {
                var signalGroup = ServerSignalName(serverId);
                var data = new dynamic[] { serverId, sub };
                await Clients.Group(signalGroup).SubscriptionDeleted(data);
            })
            .InspectErrAsync(err => Clients.Caller.SubscriptionDeleted(err));
}
