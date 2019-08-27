using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Signal.Models;

namespace BurstChat.Signal.Hubs.Chat
{
    /// <summary>
    ///   This interface provides a contract for a class that represents all available methods that a client can consume
    ///   in order to handle responses from the chat hub.
    /// </summary>
    public interface IChatClient
    {
        /// <summary>
        ///   The caller is informed about all messages posted on a private group.
        /// </summary>
        /// <param name="messages">The messages of the group that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task AllPrivateGroupMessages(Payload<IEnumerable<Message>> messages);

        /// <summary>
        ///   The calles is informed about an error occured while trying to fetch all messages
        ///   posted on a group.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task AllPrivateGroupMessages(Payload<Error> error);

        /// <summary>
        ///   All users are informed for a new message based on the provided parameter.
        /// </summary>
        /// <param name="message">The message to be sent to connected users</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageReceived(Payload<Message> message);

        /// <summary>
        ///   The caller is informed that the message he sent wasn't posted and received by the other members
        ///   of the group.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageReceived(Payload<Error> error);

        /// <summary>
        ///   All users are informed for an edit to an existing message of a group.
        /// </summary>
        /// <param name="message">The message that was edited and will be sent to connected users</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageEdited(Payload<Message> message);

        /// <summary>
        ///   The caller is informed that an edit to an existing message could not be sent to a group.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageEdited(Payload<Error> error);

        /// <summary>
        ///   All users are informed that an existing message of a group was deleted.
        /// </summary>
        /// <param name="message">The message that was deleted and will be sent to connected users</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageDeleted(Payload<Message> message);

        /// <summary>
        ///   The caller is informed that a delete to an existing message could not be sent to a group.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageDeleted(Payload<Error> error);

        /// <summary>
        ///   The caller is informed about all messages posted on a channel.
        /// </summary>
        /// <param name="messages">The messages posted</param>
        /// <returns>A task instance</returns>
        Task AllChannelMessagesReceived(Payload<IEnumerable<Message>> messages);

        /// <summary>
        ///   The caller is informed of an error that prevented him from receiving the messages of
        ///   a channel.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A task instance</returns>
        Task AllChannelMessagesReceived(Payload<Error> error);

        /// <summary>
        ///   All users of a channel are informed that a new message was posted.
        /// </summary>
        /// <param name="message">The message that was posted</param>
        /// <returns>A task instance</returns>
        Task ChannelMessageReceived(Payload<Message> message);

        /// <summary>
        ///   The caller is informed of an error when posting a new message to a channel.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A task instance</returns>
        Task ChannelMessageReceived(Payload<Error> error);

        /// <summary>
        ///   All users of a channel are informed that a message was edited.
        /// </summary>
        /// <param name="message">The message that was edited</param>
        /// <returns>A task instance</returns>
        Task ChannelMessageEdited(Payload<Message> message);

        /// <summary>
        ///   The caller is informed of an error that occured while editing a message of a channel.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A task instance</returns>
        Task ChannelMessageEdited(Payload<Error> error);

        /// <summary>
        ///   All users of a channel are informed of a message that was deleted.
        /// </summary>
        /// <param name="message">The message that was deleted</param>
        /// <returns>A task instance</returns>
        Task ChannelMessageDeleted(Payload<Message> message);

        /// <summary>
        ///   The caller is informed that an error occured while deleting a message of a channel.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A task instance</returns>
        Task ChannelMessageDeleted(Payload<Error> error);
    }
}
