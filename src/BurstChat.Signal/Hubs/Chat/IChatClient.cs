using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Models;

namespace BurstChat.Signal.Hubs.Chat
{
    /// <summary>
    /// This interface provides a contract for a class that represents all available methods that a client can consume
    /// in order to handle responses from the chat hub.
    /// </summary>
    public interface IChatClient
    {
        /// <summary>
        /// Informs the caller that the new server that was requested has been created successfully.
        /// </summary>
        /// <param name="server">The server instance that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task AddedServer(Server server);

        /// <summary>
        /// informs the caller that the new server that was requested could not be created.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task AddedServer(Error error);

        /// <summary>
        /// Informs the members of a server that a user was removed.
        /// </summary>
        /// <param name="data">The server id and subscription instance</param>
        /// <returns>A task instance</returns>
        Task SubscriptionDeleted(dynamic[] data);

        /// <summary>
        /// Informs the caller that the removal of a user from a server was not successful.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task SubscriptionDeleted(Error error);

        /// <summary>
        /// Informs the caller of all the invitations sent to him.
        /// </summary>
        /// <param name="invitations">The server invitations that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task Invitations(IEnumerable<Invitation> invitations);

        /// <summary>
        /// The called is informed about an error occured while trying to fetch all invitations
        /// sent to him.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task Invitations(Error error);

        /// <summary>
        /// Informs a user about a new invitation that was sent to him.
        /// </summary>
        /// <param name="invitation">The server invitation that will be delivered to the user</param>
        /// <returns>A task instance</returns>
        Task NewInvitation(Invitation invitation);

        /// <summary>
        /// Informs the caller about an error that occured while trying to send a new invitation.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task NewInvitation(Error error);

        /// <summary>
        /// Informs all users of a server that a new invitation was accepted by a user.
        /// </summary>
        /// <param name="invitation">The invitation that will be delivered to the users</param>
        /// <returns>A task instance</returns>
        Task UpdatedInvitation(Invitation invitation);

        /// <summary>
        /// Informs the caller about an error that occured while trying to modify an invitation.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task UpdatedInvitation(Error error);

        /// <summary>
        /// Informs the caller that his connection id was added to a signal group.
        /// </summary>
        Task SelfAddedToPrivateGroup();

        /// <summary>
        /// The caller is informed about all messages posted on a private group.
        /// </summary>
        /// <param name="messages">The messages of the group that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task AllPrivateGroupMessages(Payload<IEnumerable<Message>> messages);

        /// <summary>
        /// The calles is informed about an error occured while trying to fetch all messages
        /// posted on a group.
        /// </summary>
        /// <param name="error">The error that will be delivered to the caller</param>
        /// <returns>A task instance</returns>
        Task AllPrivateGroupMessages(Payload<Error> error);

        /// <summary>
        /// All users are informed for a new message based on the provided parameter.
        /// </summary>
        /// <param name="message">The message to be sent to connected users</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageReceived(Payload<Message> message);

        /// <summary>
        /// The caller is informed that the message he sent wasn't posted and received by the other members
        /// of the group.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageReceived(Payload<Error> error);

        /// <summary>
        /// All users are informed for an edit to an existing message of a group.
        /// </summary>
        /// <param name="message">The message that was edited and will be sent to connected users</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageEdited(Payload<Message> message);

        /// <summary>
        /// The caller is informed that an edit to an existing message could not be sent to a group.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageEdited(Payload<Error> error);

        /// <summary>
        /// All users are informed that an existing message of a group was deleted.
        /// </summary>
        /// <param name="message">The message that was deleted and will be sent to connected users</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageDeleted(Payload<Message> message);

        /// <summary>
        /// The caller is informed that a delete to an existing message could not be sent to a group.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task PrivateGroupMessageDeleted(Payload<Error> error);

        /// <summary>
        /// The users of a server are informed of a new channel that was created.
        /// </summary>
        /// <param name="data">A tuple of the server id and the new channel</param>
        /// <returns>A task instance</returns>
        Task ChannelCreated(dynamic[] data);

        /// <summary>
        /// The caller is informed that the creation of a new channel was not successful.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task ChannelCreated(Error error);

        /// <summary>
        /// The users of a server are informed of a channel whose info changed.
        /// </summary>
        /// <param name="channel">The channel instance that will be delivered to clients.</param>
        /// <returns>A task instance</returns>
        Task ChannelUpdated(Channel channel);

        /// <summary>
        /// The caller is informed that the update of a channel was not completed successfully.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task ChannelUpdated(Error error);

        /// <summary>
        /// The users of a server are informed of a channel that was removed.
        /// </summary>
        /// <param name="channelId">The id of the channel that was removed</param>
        /// <returns>A task instance</returns>
        Task ChannelDeleted(int channelId);

        /// <summary>
        /// The caller is informed that a channel could not be deleted.
        /// </summary>
        /// <param name="error">The error to be sent to the caller</param>
        /// <returns>A task instance</returns>
        Task ChannelDeleted(Error error);

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

        /// <summary>
        ///   Informs the caller that his connection id was added to a signal group.
        /// </summary>
        Task SelfAddedToDirectMessaging();

        /// <summary>
        ///   Informs the caller that a new direct messaging has been created and that his
        ///   connection id was added to a signal group.
        /// </summary>
        /// <param name="directMessaging">The direct messaging details</param>
        /// <returns>A task instance</returns>
        Task NewDirectMessaging(Payload<DirectMessaging> directMessaging);

        /// <summary>
        ///   The caller is informed that an error occured while the new direct messaging entry was
        ///   being created.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A task instance</returns>
        Task NewDirectMessaging(Error error);

        /// <summary>
        ///   Informs the caller of all messages posted on a direct messaging chat.
        /// </summary>
        /// <param name="messages">The messages that will delivered to the caller</param>
        /// <returns>A Task instance</returns>
        Task AllDirectMessagesReceived(Payload<IEnumerable<Message>> messages);

        /// <summary>
        ///   Informs the caller that an error occured while trying to fetch all messages
        ///   of a direct messaging chat.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A Task instance</returns>
        Task AllDirectMessagesReceived(Payload<Error> error);

        /// <summary>
        ///   Informs a direct messaging signal group about a new message that was posted.
        /// </summary>
        /// <param name="message">The message to be delivered</param>
        /// <returns>A Task instance</returns>
        Task DirectMessageReceived(Payload<Message> message);

        /// <summary>
        ///   Informs the caller that an error occured while a new message was being posted
        ///   in a direct messaging group.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A Task instance</returns>
        Task DirectMessageReceived(Payload<Error> error);

        /// <summary>
        ///   Informs a direct messaging signal group about a message that was edited.
        /// </summary>
        /// <param name="message">The message to be delivered</param>
        /// <returns>A Task instance</returns>
        Task DirectMessageEdited(Payload<Message> message);

        /// <summary>
        ///   Informs the caller that an error occured while a message was being edited
        ///   in a direct messaging group.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A Task instance</returns>
        Task DirectMessageEdited(Payload<Error> error);

        /// <summary>
        ///   Informs a direct messaging signal group about a message that was deleted.
        /// </summary>
        /// <param name="message">The message to be delivered</param>
        /// <returns>A Task instance</returns>
        Task DirectMessageDeleted(Payload<Message> message);

        /// <summary>
        ///   Informs the caller that an error occured while a message was being deleted
        ///   in a direct messaging group.
        /// </summary>
        /// <param name="error">The error that occured</param>
        /// <returns>A Task instance</returns>
        Task DirectMessageDeleted(Payload<Error> error);
    }
}
