using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Infrastructure.Errors;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Signal.Models;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat;

public partial class ChatHub
{
    private string ChannelSignalName(int id) => $"channel:{id}";

    public Task PostChannel(int serverId, Channel channel) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _channelsService.Insert(userId, serverId, channel))
            .InspectAsync(async channel =>
            {
                var signalGroup = ServerSignalName(serverId);
                await Clients
                    .Group(signalGroup)
                    .ChannelCreated(new dynamic[] { serverId, channel });
            })
            .InspectErrAsync(err => Clients.Caller.ChannelCreated(err.Into()));

    public Task PutChannel(int serverId, Channel channel) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _channelsService.Update(userId, channel))
            .InspectAsync(async channel =>
            {
                var signalGroup = ServerSignalName(serverId);
                await Clients.Group(signalGroup).ChannelUpdated(channel);
            })
            .InspectErrAsync(err => Clients.Caller.ChannelUpdated(err.Into()));

    public Task DeleteChannel(int serverId, int channelId) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _channelsService.Delete(userId, channelId))
            .InspectAsync(async channel =>
            {
                var signalGroup = ServerSignalName(serverId);
                await Clients.Group(signalGroup).ChannelDeleted(channelId);
            })
            .InspectErrAsync(err => Clients.Caller.ChannelDeleted(err.Into()));

    public Task AddToChannelConnection(int channelId) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _channelsService.Get(userId, channelId))
            .InspectAsync(async channel =>
            {
                var signalGroup = ChannelSignalName(channelId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.SelfAddedToChannel();
            });

    public Task GetAllChannelMessages(
        int channelId,
        string? searchTerm = null,
        long? lastMessageId = null
    )
    {
        var signalGroup = ChannelSignalName(channelId);

        return Context
            .GetHttpContext()
            .GetUserId()
            .And(userId =>
                _channelsService.GetMessages(userId, channelId, searchTerm, lastMessageId)
            )
            .InspectAsync(async messages =>
            {
                var payload = new Payload<IEnumerable<Message>>(signalGroup, messages);
                await Clients.Caller.AllChannelMessagesReceived(payload);
            })
            .InspectErrAsync(async err =>
            {
                var errorPayload = new Payload<Error>(signalGroup, err.Into());
                await Clients.Caller.AllChannelMessagesReceived(errorPayload);
            });
    }

    public Task PostChannelMessage(int channelId, Message message)
    {
        var signalGroup = ChannelSignalName(channelId);

        return Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _channelsService.InsertMessage(userId, channelId, message))
            .InspectAsync(async message =>
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).ChannelMessageReceived(payload);
            })
            .InspectErrAsync(async err =>
            {
                var errorPayload = new Payload<Error>(signalGroup, err.Into());
                await Clients.Caller.ChannelMessageReceived(errorPayload);
            });
    }

    public Task PutChannelMessage(int channelId, Message message)
    {
        var signalGroup = ChannelSignalName(channelId);

        return Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _channelsService.UpdateMessage(userId, channelId, message))
            .InspectAsync(async message =>
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).ChannelMessageEdited(payload);
            })
            .InspectErrAsync(async err =>
            {
                var errorPayload = new Payload<Error>(signalGroup, err.Into());
                await Clients.Caller.ChannelMessageEdited(errorPayload);
            });
    }

    public Task DeleteChannelMessage(int channelId, Message message)
    {
        var signalGroup = ChannelSignalName(channelId);

        return Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _channelsService.DeleteMessage(userId, channelId, message.Id))
            .InspectAsync(async message =>
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).ChannelMessageDeleted(payload);
            })
            .InspectErrAsync(async err =>
            {
                var errorPayload = new Payload<Error>(signalGroup, err.Into());
                await Clients.Caller.ChannelMessageDeleted(errorPayload);
            });
    }
}
