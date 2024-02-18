using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Monads;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Models;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat;

public partial class ChatHub
{
    private string DirectMessagingName(long id) => $"dm:{id}";

    public Task AddToDirectMessaging(long directMessagingId) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _directMessagingService.Get(userId, directMessagingId))
            .InspectAsync(async dm =>
            {
                var signalGroup = DirectMessagingName(directMessagingId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.SelfAddedToDirectMessaging();
            });

    public Task PostNewDirectMessaging(long firstParticipantId, long secondParticipantId) =>
        Context
            .GetHttpContext()
            .GetUserId()
            .And(userId =>
                _directMessagingService
                    .Get(userId, firstParticipantId, secondParticipantId)
                    .Or(() => _directMessagingService.Insert(userId, firstParticipantId, secondParticipantId))
            )
            .InspectAsync(async dm =>
            {
                var signalGroup = DirectMessagingName(dm.Id);
                var payload = new Payload<DirectMessaging>(signalGroup, dm);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.NewDirectMessaging(payload);
            })
            .InspectErrAsync(err => Clients.Caller.NewDirectMessaging(err));

    public async Task GetAllDirectMessages(long directMessagingId, string? searchTerm, long? lastMessageId)
    {
        var signalGroup = DirectMessagingName(directMessagingId);

        await Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _directMessagingService.GetMessages(userId, directMessagingId, searchTerm, lastMessageId))
            .InspectAsync(async dm =>
            {
                var payload = new Payload<IEnumerable<Message>>(signalGroup, dm);
                await Clients.Caller.AllDirectMessagesReceived(payload);
            })
            .InspectErrAsync(async err =>
            {
                var errorPayload = new Payload<MonadException>(signalGroup, err);
                await Clients.Caller.AllDirectMessagesReceived(errorPayload);
            });
    }

    public async Task PostDirectMessage(long directMessagingId, Message message)
    {
        var signalGroup = DirectMessagingName(directMessagingId);

        await Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _directMessagingService.InsertMessage(userId, directMessagingId, message))
            .InspectAsync(async dm =>
            {
                var payload = new Payload<Message>(signalGroup, dm);
                await Clients.Groups(signalGroup).DirectMessageReceived(payload);
            })
            .InspectErrAsync(async err =>
            {
                var errorPayload = new Payload<MonadException>(signalGroup, err);
                await Clients.Groups(signalGroup).DirectMessageReceived(errorPayload);
            });
    }

    public async Task PutDirectMessage(long directMessageId, Message message)
    {
        var signalGroup = DirectMessagingName(directMessageId);

        await Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _directMessagingService.UpdateMessage(userId, directMessageId, message))
            .InspectAsync(async dm =>
            {
                var payload = new Payload<Message>(signalGroup, dm);
                await Clients.Groups(signalGroup).DirectMessageEdited(payload);
            })
            .InspectErrAsync(async err =>
            {
                var errorPayload = new Payload<MonadException>(signalGroup, err);
                await Clients.Caller.DirectMessageEdited(errorPayload);
            });
    }

    public async Task DeleteDirectMessage(long directMessageId, Message message)
    {
        var signalGroup = DirectMessagingName(directMessageId);

        await Context
            .GetHttpContext()
            .GetUserId()
            .And(userId => _directMessagingService.DeleteMessage(userId, directMessageId, message.Id))
            .InspectAsync(async dm =>
            {
                var payload = new Payload<Message>(signalGroup, dm);
                await Clients.Groups(signalGroup).DirectMessageDeleted(payload);
            })
            .InspectErrAsync(async err =>
            {
                var errorPayload = new Payload<MonadException>(signalGroup, err);
                await Clients.Caller.DirectMessageDeleted(errorPayload);
            });
    }
}
