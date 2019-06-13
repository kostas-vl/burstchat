using System;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Shared.Schema.Users;
using Microsoft.EntityFrameworkCore;

namespace BurstChat.Shared.Context
{
    public class BurstChatContext : DbContext
    {
        public DbSet<Server> Servers
        {
            get; set;
        }

        public DbSet<User> Users
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

        }
    }
}