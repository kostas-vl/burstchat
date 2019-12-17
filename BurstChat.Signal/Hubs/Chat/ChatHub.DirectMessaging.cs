using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Users;
using BurstChat.Signal.Models;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat
{
    public partial class ChatHub
    {
        /// <summary>
        ///   Constructs the appropriate signal direct messaging name of all users connected
        ///   to a dm chat.
        /// </summary>
        /// <param name="id">The id of the direct messaging entry</param>
        /// <returns>The string signal group name</returns>
        private string DirectMessagingName(long id) => $"dm:{id}";

        /// <summary>
        ///   Adds a new connection to a signalr group based on the provided direct messaging id.
        /// </summary>
        /// <param name="directMessagingId">The id of the target direct messaging entry</param>
        /// <returns>A task instance</returns>
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

        /// <summary>
        ///   Adds a new direct messaging entry, if it does not already exist, and also assings the callers
        ///   connection id to a signalr group based on the resulting direct messaging id.
        /// </summary>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the second participant</param>
        /// <returns>A Task instance</returns>
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

        /// <summary>
        ///   Informs the caller of all messages posted to a direct messaging chat.
        /// </summary>
        /// <param name="directMessagingId">The id of the target direct messaging chat</param>
        /// <param name="lastMessageId">The message id from which all previous messages will be fetched</param>
        /// <returns>A Task instance</returns>
        public async Task GetAllDirectMessages(long directMessagingId, long? lastMessageId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _directMessagingService.GetMessagesAsync(httpContext, directMessagingId, lastMessageId);
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

        /// <summary>
        ///   Informs a direct messaging chat about a new message from a user.
        /// </summary>
        /// <param name="directMessagingId">The id of the direct messaging chat</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A Task instance</returns>
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

        /// <summary>
        ///   Informs a direct messaging chat about a message that was edited from a user.
        /// </summary>
        /// <param name="directMessageId">The id of the direct messaging chat</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>A Task instance</returns>
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

        /// <summary>
        ///   Informs a direct messaging chat about a message that was deleted from a user.
        /// </summary>
        /// <param name="directMessageId">The id of the direct messaging chat</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>A Task instance</returns>
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
}
