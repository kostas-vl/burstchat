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
    private string DirectMessagingName(long id) => $"dm:{id}";

    public async Task AddToDirectMessaging(long directMessagingId)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _directMessagingService.GetAsync(httpContext, directMessagingId);

        if (monad is Success<DirectMessaging, Error> success)
        {
            var signalGroup = DirectMessagingName(directMessagingId);
            await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
            await Clients.Caller.SelfAddedToDirectMessaging();
        }
    }

    public async Task PostNewDirectMessaging(long firstParticipantId, long secondParticipantId)
    {
        // Trying to find an existing direct messaging entry based on the users provided.
        var httpContext = Context.GetHttpContext();
        var getMonad = await _directMessagingService.GetAsync(httpContext, firstParticipantId, secondParticipantId);

        if (getMonad is Success<DirectMessaging, Error> success)
        {
            var signalGroup = DirectMessagingName(success.Value.Id);
            var payload = new Payload<DirectMessaging>(signalGroup, success.Value);
            await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
            await Clients.Caller.NewDirectMessaging(payload);
        }

        // Creates a new direct messaging entry between two users.
        var directMessaging = new DirectMessaging
        {
            FirstParticipantUserId = firstParticipantId,
            SecondParticipantUserId = secondParticipantId
        };
        var monad = await _directMessagingService.PostAsync(httpContext, directMessaging);

        switch (monad)
        {
            case Success<DirectMessaging, Error> postSuccess:
                var signalGroup = DirectMessagingName(postSuccess.Value.Id);
                var payload = new Payload<DirectMessaging>(signalGroup, postSuccess.Value);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.NewDirectMessaging(payload);
                break;

            case Failure<DirectMessaging, Error> failure:
                await Clients.Caller.NewDirectMessaging(failure.Value);
                break;

            default:
                await Clients.Caller.NewDirectMessaging(SystemErrors.Exception());
                break;
        }
    }

    public async Task GetAllDirectMessages(long directMessagingId, string? searchTerm, long? lastMessageId)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _directMessagingService.GetMessagesAsync(httpContext,
                                                                   directMessagingId,
                                                                   searchTerm,
                                                                   lastMessageId);
        var signalGroup = DirectMessagingName(directMessagingId);

        switch (monad)
        {
            case Success<IEnumerable<Message>, Error> success:
                var payload = new Payload<IEnumerable<Message>>(signalGroup, success.Value);
                await Clients.Caller.AllDirectMessagesReceived(payload);
                break;

            case Failure<IEnumerable<Message>, Error> failure:
                var errorPayload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.AllDirectMessagesReceived(errorPayload);
                break;

            default:
                var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.AllDirectMessagesReceived(exceptionPayload);
                break;
        }
    }

    public async Task PostDirectMessage(long directMessagingId, Message message)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _directMessagingService.PostMessageAsync(httpContext, directMessagingId, message);
        var signalGroup = DirectMessagingName(directMessagingId);

        switch (monad)
        {
            case Success<Message, Error> success:
            var payload = new Payload<Message>(signalGroup, success.Value);
            await Clients.Groups(signalGroup).DirectMessageReceived(payload);
                break;

            case Failure<Message, Error> failure:
                var errorPayload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Groups(signalGroup).DirectMessageReceived(errorPayload);
                break;

            default:
                var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Groups(signalGroup).DirectMessageReceived(exceptionPayload);
                break;
        }
    }

    public async Task PutDirectMessage(long directMessageId, Message message)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _directMessagingService.PutMessageAsync(httpContext, directMessageId, message);
        var signalGroup = DirectMessagingName(directMessageId);

        switch (monad)
        {
            case Success<Message, Error> success:
                var payload = new Payload<Message>(signalGroup, success.Value);
                await Clients.Groups(signalGroup).DirectMessageEdited(payload);
                break;

            case Failure<Message, Error> failure:
                var errorPayload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.DirectMessageEdited(errorPayload);
                break;

            default:
                var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.DirectMessageEdited(exceptionPayload);
                break;
        }
    }

    public async Task DeleteDirectMessage(long directMessageId, Message message)
    {
        var httpContext = Context.GetHttpContext();
        var monad = await _directMessagingService.DeleteMessageAsync(httpContext, directMessageId, message);
        var signalGroup = DirectMessagingName(directMessageId);

        switch (monad)
        {
            case Success<Message, Error> success:
                var payload = new Payload<Message>(signalGroup, success.Value);
                await Clients.Groups(signalGroup).DirectMessageDeleted(payload);
                break;

            case Failure<Message, Error> failure:
                var errorPayload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.DirectMessageDeleted(errorPayload);
                break;

            default:
                var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.DirectMessageDeleted(exceptionPayload);
                break;
        }
    }
}
