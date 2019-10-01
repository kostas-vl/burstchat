#nullable disable

using System;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Shared.Schema.Users;
using Microsoft.EntityFrameworkCore;

namespace BurstChat.Shared.Context
{
    /// <summary>
    ///   This class represents the burst chat database context and all of its tables.
    /// </summary>
    public class BurstChatContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public DbSet<Link> Links { get; set; }

        public DbSet<Server> Servers { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<ChannelDetails> ChannelDetails { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<PrivateGroup> PrivateGroups { get; set; }

        public DbSet<DirectMessaging> DirectMessaging { get; set; }

        public DbSet<OneTimePassword> OneTimePassword { get; set; }

        /// <summary>
        ///   Executes the necessary start up code for the burst chat database context.
        /// </summary>
        public BurstChatContext(DbContextOptions<BurstChatContext> options)
            : base(options) { }
    }
}
