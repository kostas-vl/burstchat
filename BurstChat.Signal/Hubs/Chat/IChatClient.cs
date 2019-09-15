using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
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
        ///     Informs the caller that the new server that was requested has been created successfully.
        /// </summary>
        /// <param name="server">The server instance that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task AddedServer(Server server);

        /// <summary>
        ///     informs the caller that the new server that was requested could not be created.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task AddedServer(Error error);

        /// <summary>
        ///     Informs the caller of all the invitations sent to him.
        /// </summary>
        /// <param name="invitations">The server invitations that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task Invitations(IEnumerable<Invitation> invitations);

        /// <summary>
        ///     The called is informed about an error occured while trying to fetch all invitations
        ///     sent to him.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task Invitations(Error error);

        /// <summary>
        ///     Informs a user about a new invitation that was sent to him.
        /// </summary>
        /// <param name="invitation">The server invitation that will be delivered to the user</param>
        /// <returns>A task instance</returns>
        Task NewInvitation(Invitation invitation);

        /// <summary>
        ///     Informs the caller about an error that occured while trying to send a new invitation.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task NewInvitation(Error error);

        /// <summary>
        ///     Informs all users of a server that a new invitation was accepted by a user.
        /// </summary>
        /// <param name="invitation">The invitation that will be delivered to the users</param>
        /// <returns>A task instance</returns>
        Task UpdatedInvitation(Invitation invitation);

        /// <summary>
        ///     Informs the caller about an error that occured while trying to modify an invitation.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task UpdatedInvitation(Error error);

        /// <summary>
        ///   Informs the caller that his connection id was added to a signal group.
        /// </summary>
        Task SelfAddedToPrivateGroup();

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
        ///   Informs the caller that his connection id was added to a signal group.
        /// </summary>
        Task SelfAddedToChannel();

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
