using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Servers;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat;

public partial class ChatHub
{
    private string ServerSignalName(int id) => $"server:{id}";

    public async Task AddServer(Server server)
    {
        var httpContext = Context.GetHttpContext();
        var sub = Context;
        var monad = await _serverService.PostAsync(httpContext, server);

        switch (monad)
        {
            case Success<Server, Error> success:
                await Clients.Caller.AddedServer(success.Value);
                break;

            case Failure<Server, Error> failure:
                await Clients.Caller.AddedServer(failure.Value);
                break;

            default:
                await Clients.Caller.AddedServer(SystemErrors.Exception());
                break;
        }
    }

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

    public async Task UpdateServerInfo(int serverId)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _serverService.GetAsync(httpContext, serverId);

        switch (monad)
        {
            case Success<Server, Error> success:
                var signalGroup = ServerSignalName(serverId);
                await Clients.Group(signalGroup).UpdatedServer(success.Value);
                break;

            case Failure<Server, Error> failure:
                await Clients.Caller.UpdatedServer(failure.Value);
                break;

            default:
                await Clients.Caller.UpdatedServer(SystemErrors.Exception());
                break;
        }
    }

    public async Task DeleteSubscription(int serverId, Subscription subscription)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _serverService.DeleteSubscription(httpContext, serverId, subscription);

        switch (monad)
        {
            case Success<Subscription, Error> success:
                var signalGroup = ServerSignalName(serverId);
                var data = new dynamic[] { serverId, subscription };
                await Clients.Group(signalGroup).SubscriptionDeleted(data);
                break;

            case Failure<Subscription, Error> failure:
                await Clients.Caller.SubscriptionDeleted(failure.Value);
                break;

            default:
                await Clients.Caller.SubscriptionDeleted(SystemErrors.Exception());
                break;
        }
    }
}
