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
        /// Constructs the appropriate signal group name for a channel chat.
        /// </summary>
        /// <param name="id">The id of the channel</param>
        /// <returns>The string signal channel name</param>
        private string ChannelSignalName(int id) => $"channel:{id}";

        /// <summary>
        /// Creates a new channel for a server.
        /// </summary>
        /// <param name="serverId">The id of the server that the channel belongs</param>
        /// <param name="channel">The instance of the channel to be created</param>
        /// <returns>A task instance</returns>
        public async Task PostChannel(int serverId, Channel channel)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.PostAsync(httpContext, serverId, channel);

            switch(monad)
            {
                case Success<Channel, Error> success:
                    var signalGroup = ServerSignalName(serverId);
                    await Clients.Group(signalGroup).ChannelCreated(new dynamic[] { serverId, success.Value });
                    break;

                case Failure<Channel, Error> failure:
                    await Clients.Caller.ChannelCreated(failure.Value);
                    break;

                default:
                    await Clients.Caller.ChannelCreated(SystemErrors.Exception());
                    break;
            }
        }

        /// <summary>
        /// Updates a channel of a server.
        /// </summary>
        /// <param name="serverId">The id of the server that the channel belongs</param>
        /// <param name="channel">The instance of the channel to updated</param>
        /// <returns>A task instance</returns>
        public async Task PutChannel(int serverId, Channel channel)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.PutAsync(httpContext, channel);

            switch (monad)
            {
                case Success<Channel, Error> success:
                    var signalGroup = ServerSignalName(serverId);
                    await Clients.Group(signalGroup).ChannelUpdated(success.Value);
                    break;

                case Failure<Channel, Error> failure:
                    await Clients.Caller.ChannelUpdated(failure.Value);
                    break;

                default:
                    await Clients.Caller.ChannelUpdated(SystemErrors.Exception());
                    break;
            }
        }

        /// <summary>
        /// Removes a channel from a server.
        /// </summary>
        /// <param name="serverId">The id of the server that the channel belongs</param>
        /// <param name="channelId">The id of the channel to be deleted</param>
        /// <returns>A task instance</returns>
        public async Task DeleteChannel(int serverId, int channelId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.DeleteAsync(httpContext, channelId);

            switch (monad)
            {
                case Success<Unit, Error> _:
                    var signalGroup = ServerSignalName(serverId);
                    await Clients.Group(signalGroup).ChannelDeleted(channelId);
                    break;

                case Failure<Unit, Error> failure:
                    await Clients.Caller.ChannelDeleted(failure.Value);
                    break;

                default:
                    await Clients.Caller.ChannelDeleted(SystemErrors.Exception());
                    break;
            }
        }

        /// <summary>
        /// Adds a new connection to a signalr group based on the provided channel id.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>A task instance</returns>
        public async Task AddToChannelConnection(int channelId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.GetAsync(httpContext, channelId);

            if (monad is Success<Channel, Error>)
            {
                var signalGroup = ChannelSignalName(channelId);
                await Groups.AddToGroupAsync(Context.ConnectionId, signalGroup);
                await Clients.Caller.SelfAddedToChannel();
            }
        }

        /// <summary>
        /// Informs the caller of all messages posted to a channel.
        /// </summary>
        /// <param name="channelId">The id of the target channel</param>
        /// <returns>A task instance</returns>
        public async Task GetAllChannelMessages(int channelId)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.GetMessagesAsync(httpContext, channelId);
            var signalGroup = ChannelSignalName(channelId);

            switch (monad)
            {
                case Success<IEnumerable<Message>, Error> success:
                    var payload = new Payload<IEnumerable<Message>>(signalGroup, success.Value);
                    await Clients.Groups(signalGroup).AllChannelMessagesReceived(payload);
                    break;

                case Failure<IEnumerable<Message>, Error> failure:
                    var errorPayload = new Payload<Error>(signalGroup, failure.Value);
                    await Clients.Caller.AllChannelMessagesReceived(errorPayload);
                    break;

                default:
                    var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                    await Clients.Caller.AllChannelMessagesReceived(exceptionPayload);
                    break;
            }
        }

        /// <summary>
        /// Informs the users of a channel about a new message that was posted.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task instance</returns>
        public async Task PostChannelMessage(int channelId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.PostMessageAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            switch (monad)
            {
                case Success<Message, Error> _:
                    var payload = new Payload<Message>(signalGroup, message);
                    await Clients.Groups(signalGroup).ChannelMessageReceived(payload);
                    break;

                case Failure<Message, Error> failure:
                    var errorPayload = new Payload<Error>(signalGroup, failure.Value);
                    await Clients.Caller.ChannelMessageReceived(errorPayload);
                    break;

                default:
                    var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                    await Clients.Caller.ChannelMessageReceived(exceptionPayload);
                    break;
            }
        }

        /// <summary>
        /// Informs the users of a channel that a message was edited.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task instance</returns>
        public async Task PutChannelMessage(int channelId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.PutMessageAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            switch (monad)
            {
                case Success<Unit, Error> _:
                    var payload = new Payload<Message>(signalGroup, message);
                    await Clients.Groups(signalGroup).ChannelMessageEdited(payload);
                    break;

                case Failure<Unit, Error> failure:
                    var errorPayload = new Payload<Error>(signalGroup, failure.Value);
                    await Clients.Caller.ChannelMessageEdited(errorPayload);
                    break;

                default:
                    var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                    await Clients.Caller.ChannelMessageEdited(exceptionPayload);
                    break;
            }
        }

        /// <summary>
        /// Informs the users of a channel that a message was deleted.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task instance</returns>
        public async Task DeleteChannelMessage(int channelId, Message message)
        {
            var httpContext = Context.GetHttpContext();
            var monad = await _channelsService.DeleteMessageAsync(httpContext, channelId, message);
            var signalGroup = ChannelSignalName(channelId);

            switch (monad)
            {
                case Success<Unit, Error> _:
                    var payload = new Payload<Message>(signalGroup, message);
                    await Clients.Groups(signalGroup).ChannelMessageDeleted(payload);
                    break;

                case Failure<Unit, Error> failure:
                    var errorPayload = new Payload<Error>(signalGroup, failure.Value);
                    await Clients.Caller.ChannelMessageDeleted(errorPayload);
                    break;

                default:
                    var exceptionPayload = new Payload<Error>(signalGroup, SystemErrors.Exception());
                    await Clients.Caller.ChannelMessageDeleted(exceptionPayload);
                    break;
            }
        }
    }
}
