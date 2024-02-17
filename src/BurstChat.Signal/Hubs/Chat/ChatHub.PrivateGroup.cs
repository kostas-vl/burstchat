using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Models;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat;

public partial class ChatHub
{
    private string PrivateGroupSignalName(long id) => $"privateGroup:{id}";

    public async Task AddToPrivateGroupConnection(long groupId)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _privateGroupMessagingService.GetPrivateGroupAsync(httpContext, groupId);

        if (monad is Success<PrivateGroup, Error>)
        {
            var signalGroup = PrivateGroupSignalName(groupId);
            await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
            await Clients.Caller.SelfAddedToPrivateGroup();
        }
    }

    public async Task GetAllPrivateGroupMessages(long groupId)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _privateGroupMessagingService.GetAllAsync(httpContext, groupId);
        var signalGroup = PrivateGroupSignalName(groupId);

        switch (monad)
        {
            case Success<IEnumerable<Message>, Error> success:
                var payload = new Payload<IEnumerable<Message>>(signalGroup, success.Value);
                await Clients.Caller.AllPrivateGroupMessages(payload);
                break;

            case Failure<IEnumerable<Message>, Error> failure:
                var failurePayload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.AllPrivateGroupMessages(failurePayload);
                break;

            default:
                var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.AllPrivateGroupMessages(exceptionPayload);
                break;
        }
    }

    public async Task PostPrivateGroupMessage(long groupId, Message message)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _privateGroupMessagingService.PostAsync(httpContext, groupId, message);
        var signalGroup = PrivateGroupSignalName(groupId);

        switch (monad)
        {
            case Success<Unit, Error> _:
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Group(signalGroup).PrivateGroupMessageReceived(payload);
                break;

            case Failure<Unit, Error> failure:
                var failurePayload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.PrivateGroupMessageReceived(failurePayload);
                break;

            default:
                var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.PrivateGroupMessageReceived(exceptionPayload);
                break;
        }
    }

    public async Task PutPrivateGroupMessage(long groupId, Message message)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _privateGroupMessagingService.PostAsync(httpContext, groupId, message);
        var signalGroup = PrivateGroupSignalName(groupId);

        switch (monad)
        {
            case Success<Unit, Error> _:
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).PrivateGroupMessageEdited(payload);
                break;

            case Failure<Unit, Error> failure:
                var failurePayload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.PrivateGroupMessageEdited(failurePayload);
                break;

            default:
                var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.PrivateGroupMessageEdited(exceptionPayload);
                break;
        }
    }

    public async Task DeletePrivateGroupMessage(long groupId, Message message)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _privateGroupMessagingService.DeleteAsync(httpContext, groupId, message);
        var signalGroup = PrivateGroupSignalName(groupId);

        switch (monad)
        {
            case Success<Unit, Error> _:
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).PrivateGroupMessageDeleted(payload);
                break;

            case Failure<Unit, Error> failure:
                var failurePayload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.PrivateGroupMessageDeleted(failurePayload);
                break;

            default:
                var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.PrivateGroupMessageDeleted(exceptionPayload);
                break;
        }
    }
}
