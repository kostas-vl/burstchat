using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Monads;
using BurstChat.Infrastructure.Errors;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Signal.Models;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat;

public partial class ChatHub
{
    private string PrivateGroupSignalName(long id) => $"privateGroup:{id}";

    public Task AddToPrivateGroupConnection(long groupId) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _privateGroupService.Get(userId, groupId))
            .InspectAsync(async group =>
            {
                var signalGroup = PrivateGroupSignalName(groupId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.SelfAddedToPrivateGroup();
            });

    public Task GetAllPrivateGroupMessages(long groupId)
    {
        var signalGroup = PrivateGroupSignalName(groupId);

        return Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _privateGroupService.GetMessages(userId, groupId))
            .InspectAsync(async messages =>
            {
                var payload = new Payload<IEnumerable<Message>>(signalGroup, messages);
                await Clients.Caller.AllPrivateGroupMessages(payload);
            })
            .InspectErrAsync(async err =>
            {
                var failurePayload = new Payload<Error>(signalGroup, err.Into());
                await Clients.Caller.AllPrivateGroupMessages(failurePayload);
            });
    }

    public Task PostPrivateGroupMessage(long groupId, Message message)
    {
        var signalGroup = PrivateGroupSignalName(groupId);

        return Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _privateGroupService.InsertMessage(userId, groupId, message))
            .InspectAsync(async message =>
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Group(signalGroup).PrivateGroupMessageReceived(payload);
            })
            .InspectErrAsync(async err =>
            {
                var failurePayload = new Payload<Error>(signalGroup, err.Into());
                await Clients.Caller.PrivateGroupMessageReceived(failurePayload);
            });
    }

    public Task PutPrivateGroupMessage(long groupId, Message message)
    {
        var signalGroup = PrivateGroupSignalName(groupId);

        return Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _privateGroupService.UpdateMessage(userId, groupId, message))
            .InspectAsync(async message =>
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).PrivateGroupMessageEdited(payload);
            })
            .InspectErrAsync(async err =>
            {
                var failurePayload = new Payload<Error>(signalGroup, err.Into());
                await Clients.Caller.PrivateGroupMessageEdited(failurePayload);
            });
    }

    public Task DeletePrivateGroupMessage(long groupId, Message message)
    {
        var signalGroup = PrivateGroupSignalName(groupId);

        return Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _privateGroupService.DeleteMessage(userId, groupId, message.Id))
            .InspectAsync(async _ =>
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).PrivateGroupMessageDeleted(payload);
            })
            .InspectErrAsync(async err =>
            {
                var failurePayload = new Payload<Error>(signalGroup, err.Into());
                await Clients.Caller.PrivateGroupMessageDeleted(failurePayload);
            });
    }
}
