using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Models;

namespace BurstChat.Signal.Hubs.Chat;

public interface IChatClient
{
    Task AddedServer(Server server);

    Task AddedServer(MonadException error);

    Task UpdatedServer(Server server);

    Task UpdatedServer(MonadException error);

    Task SubscriptionDeleted(dynamic[] data);

    Task SubscriptionDeleted(MonadException error);

    Task UserUpdated(User user);

    Task UserUpdated(MonadException ex);

    Task Invitations(IEnumerable<Invitation> invitations);

    Task Invitations(MonadException error);

    Task NewInvitation(Invitation invitation);

    Task NewInvitation(MonadException error);

    Task UpdatedInvitation(Invitation invitation);

    Task UpdatedInvitation(MonadException error);

    Task SelfAddedToPrivateGroup();

    Task AllPrivateGroupMessages(Payload<IEnumerable<Message>> messages);

    Task AllPrivateGroupMessages(Payload<MonadException> error);

    Task PrivateGroupMessageReceived(Payload<Message> message);

    Task PrivateGroupMessageReceived(Payload<MonadException> error);

    Task PrivateGroupMessageEdited(Payload<Message> message);

    Task PrivateGroupMessageEdited(Payload<MonadException> error);

    Task PrivateGroupMessageDeleted(Payload<Message> message);

    Task PrivateGroupMessageDeleted(Payload<MonadException> error);

    Task ChannelCreated(dynamic[] data);

    Task ChannelCreated(MonadException error);

    Task ChannelUpdated(Channel channel);

    Task ChannelUpdated(MonadException error);

    Task ChannelDeleted(int channelId);

    Task ChannelDeleted(MonadException error);

    Task SelfAddedToChannel();

    Task AllChannelMessagesReceived(Payload<IEnumerable<Message>> messages);

    Task AllChannelMessagesReceived(Payload<MonadException> error);

    Task ChannelMessageReceived(Payload<Message> message);

    Task ChannelMessageReceived(Payload<MonadException> error);

    Task ChannelMessageEdited(Payload<Message> message);

    Task ChannelMessageEdited(Payload<MonadException> error);

    Task ChannelMessageDeleted(Payload<Message> message);

    Task ChannelMessageDeleted(Payload<MonadException> error);

    Task SelfAddedToDirectMessaging();

    Task NewDirectMessaging(Payload<DirectMessaging> directMessaging);

    Task NewDirectMessaging(MonadException error);

    Task AllDirectMessagesReceived(Payload<IEnumerable<Message>> messages);

    Task AllDirectMessagesReceived(Payload<MonadException> error);

    Task DirectMessageReceived(Payload<Message> message);

    Task DirectMessageReceived(Payload<MonadException> error);

    Task DirectMessageEdited(Payload<Message> message);

    Task DirectMessageEdited(Payload<MonadException> error);

    Task DirectMessageDeleted(Payload<Message> message);

    Task DirectMessageDeleted(Payload<MonadException> error);
}
