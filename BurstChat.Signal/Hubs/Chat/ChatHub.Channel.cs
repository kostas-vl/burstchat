using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Signal.Models;
using Microsoft.AspNetCore.SignalR;

namespace BurstChat.Signal.Hubs.Chat
{
    public partial class ChatHub
    {
        /// <summary>
        ///   Constructs the appropriate signal group name for a channel chat.
        /// </summary>
        /// <param name="id">The id of the channel</param>
        /// <returns>The string signal channel name</param>
        private string ChannelSignalName(int id) => $"channel:{id}";

        /// <summary>
        ///   Adds a new connection to a signalr group based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>A task instance</returns>
        public async Task AddToChannelConnection(int channelId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.GetChannelAsync(httpContext, channelId);

            if (monad is Success<Channel, Error>)
            {
                var signalGroup = ChannelSignalName(channelId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.SelfAddedToChannel();
            }
        }

        /// <summary>
        ///   Informs the caller of all messages posted to a channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>A task instance</returns>
        public async Task GetAllChannelMessages(int channelId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.GetAllAsync(httpContext, channelId);
            var signalGroup = ChannelSignalName(channelId);

            if (monad is Success<IEnumerable<Message>, Error> success)
            {
                var payload = new Payload<IEnumerable<Message>>(signalGroup, success.Value);
                await Clients.Caller.AllChannelMessagesReceived(payload);
            }
            else if (monad is Failure<IEnumerable<Message>, Error> failure)
            {
                var payload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.AllChannelMessagesReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.AllChannelMessagesReceived(payload);
            }
        }

        /// <summary>
        ///   Informs the users of a channel about a new message that was posted.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task instance</returns>
        public async Task PostChannelMessage(int channelId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.PostAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            if (monad is Success<Message, Error> success)
            {
                var payload = new Payload<Message>(signalGroup, success.Value);
                await Clients.Groups(signalGroup).ChannelMessageReceived(payload);
            }
            else if (monad is Failure<Message, Error> failure)
            {
                var payload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.ChannelMessageReceived(payload);
            }
            else
            {
                var payload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.ChannelMessageReceived(payload);
            }
        }

        /// <summary>
        ///   Informs the users of a channel that a message was edited.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task instance</returns>
        public async Task PutChannelMessage(int channelId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.PutAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).ChannelMessageEdited(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.ChannelMessageEdited(payload);
            }
            else
            {
                var payload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.ChannelMessageEdited(payload);
            }
        }

        /// <summary>
        ///   Informs the users of a channel that a message was deleted.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task instance</returns>
        public async Task DeleteChannelMessage(int channelId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.DeleteAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            if (monad is Success<Unit, Error> success)
            {
                var payload = new Payload<Message>(signalGroup, message);
                await Clients.Groups(signalGroup).ChannelMessageDeleted(payload);
            }
            else if (monad is Failure<Unit, Error> failure)
            {
                var payload = new Payload<Error>(signalGroup, failure.Value);
                await Clients.Caller.ChannelMessageDeleted(payload);
            }
            else
            {
                var payload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                await Clients.Caller.ChannelMessageDeleted(payload);
            }
        }
    }
}
