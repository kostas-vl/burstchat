using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Infrastructure.Errors;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Models;

namespace BurstChat.Signal.Hubs.Chat;

public interface IChatClient
{
    Task AddedServer(Server server);

    Task AddedServer(Error error);

    Task UpdatedServer(Server server);

    Task UpdatedServer(Error error);

    Task SubscriptionDeleted(dynamic[] data);

    Task SubscriptionDeleted(Error error);

    Task UserUpdated(User user);

    Task UserUpdated(Error ex);

    Task Invitations(IEnumerable<Invitation> invitations);

    Task Invitations(Error error);

    Task NewInvitation(Invitation invitation);

    Task NewInvitation(Error error);

    Task UpdatedInvitation(Invitation invitation);

    Task UpdatedInvitation(Error error);

    Task SelfAddedToPrivateGroup();

    Task AllPrivateGroupMessages(Payload<IEnumerable<Message>> messages);

    Task AllPrivateGroupMessages(Payload<Error> error);

    Task PrivateGroupMessageReceived(Payload<Message> message);

    Task PrivateGroupMessageReceived(Payload<Error> error);

    Task PrivateGroupMessageEdited(Payload<Message> message);

    Task PrivateGroupMessageEdited(Payload<Error> error);

    Task PrivateGroupMessageDeleted(Payload<Message> message);

    Task PrivateGroupMessageDeleted(Payload<Error> error);

    Task ChannelCreated(dynamic[] data);

    Task ChannelCreated(Error error);

    Task ChannelUpdated(Channel channel);

    Task ChannelUpdated(Error error);

    Task ChannelDeleted(int channelId);

    Task ChannelDeleted(Error error);

    Task SelfAddedToChannel();

    Task AllChannelMessagesReceived(Payload<IEnumerable<Message>> messages);

    Task AllChannelMessagesReceived(Payload<Error> error);

    Task ChannelMessageReceived(Payload<Message> message);

    Task ChannelMessageReceived(Payload<Error> error);

    Task ChannelMessageEdited(Payload<Message> message);

    Task ChannelMessageEdited(Payload<Error> error);

    Task ChannelMessageDeleted(Payload<Message> message);

    Task ChannelMessageDeleted(Payload<Error> error);

    Task SelfAddedToDirectMessaging();

    Task NewDirectMessaging(Payload<DirectMessaging> directMessaging);

    Task NewDirectMessaging(Error error);

    Task AllDirectMessagesReceived(Payload<IEnumerable<Message>> messages);

    Task AllDirectMessagesReceived(Payload<Error> error);

    Task DirectMessageReceived(Payload<Message> message);

    Task DirectMessageReceived(Payload<Error> error);

    Task DirectMessageEdited(Payload<Message> message);

    Task DirectMessageEdited(Payload<Error> error);

    Task DirectMessageDeleted(Payload<Message> message);

    Task DirectMessageDeleted(Payload<Error> error);
}
