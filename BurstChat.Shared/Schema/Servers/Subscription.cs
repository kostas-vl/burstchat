using System;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Shared.Schema.Servers
{
    /// <summary>
    /// This class represents a relationship of a user to a server through a user's subscription.
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// The id of the user to server subscription.
        /// </summary>
        public long Id
        {
            get; set;
        }

        /// <summary>
        /// The id of the user.
        /// </summary>
        public long UserId
        {
            get; set;
        }

        /// <summary>
        /// This is a reference navigation property for the user.
        /// </summary>
        public User User
        {
            get; set;
        }

        /// <summary>
        /// The id of the server.
        /// </summary>
        public int ServerId
        {
            get; set;
        }

        /// <summary>
        /// This is a reference navigation property for the server.
        /// </summary>
        public Server Server
        {
            get; set;
        }
    }
}
