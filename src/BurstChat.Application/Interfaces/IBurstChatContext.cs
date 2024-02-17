using BurstChat.Domain.Schema.Alpha;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;

namespace BurstChat.Application.Interfaces
{
    /// <summary>
    /// This interface contains the base contract to which any concrete implementation of the
    /// BurstChat database context must adhere to.
    /// </summary>
    public interface IBurstChatContext
    {
        DbSet<Message> Messages { get; set; }

        DbSet<Link> Links { get; set; }

        DbSet<Server> Servers { get; set; }

        DbSet<Subscription> Subscriptions { get; set; }

        DbSet<Invitation> Invitations { get; set; }

        DbSet<Channel> Channels { get; set; }

        DbSet<User> Users { get; set; }

        DbSet<PrivateGroup> PrivateGroups { get; set; }

        DbSet<DirectMessaging> DirectMessaging { get; set; }

        DbSet<OneTimePassword> OneTimePassword { get; set; }

        DbSet<AlphaInvitation> AlphaInvitations { get; set; }

        int SaveChanges();
    }
}
