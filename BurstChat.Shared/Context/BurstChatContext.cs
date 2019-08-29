using System;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Shared.Schema.Users;
using Microsoft.EntityFrameworkCore;

namespace BurstChat.Shared.Context
{
    /// <summary>
    /// This class represents the burst chat database context and all of its tables.
    /// </summary>
    public class BurstChatContext : DbContext
    {
        public DbSet<Message> Messages
        {
            get; set;
        }

        public DbSet<Server> Servers
        {
            get; set;
        }

        public DbSet<Subscription> Subscriptions
        {
            get; set;
        }

        public DbSet<Channel> Channels
        {
            get; set;
        }

        public DbSet<ChannelDetails> ChannelDetails
        {
            get; set;
        }

        public DbSet<User> Users
        {
            get; set;
        }

        public DbSet<PrivateGroupMessage> PrivateGroupMessage
        {
            get; set;
        }

        public DbSet<OneTimePassword> OneTimePassword
        {
            get; set;
        }

        /// <summary>
        /// Executes the necessary start up code for the burst chat database context.
        /// </summary>
        public BurstChatContext(DbContextOptions<BurstChatContext> options)
            : base(options) { }

        /// <summary>
        /// Overrides the base OnModelCreating in order to configure the intricate relationships between entities.
        /// </summary>
        /// <param name="builder">The model instance provided</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // setting the relationships of the Message model.
            builder
                .Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(m => m.UserId)
                .IsRequired();

            // setting the relationships of the Channel model.
            builder
                .Entity<Channel>()
                .HasOne(c => c.Details)
                .WithOne(c => c.Channel)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey<ChannelDetails>(c => c.ChannelId)
                .IsRequired();

            builder
                .Entity<Channel>()
                .HasOne(c => c.Server)
                .WithMany(s => s.Channels)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(c => c.ServerId)
                .IsRequired();
        }
    }
}
