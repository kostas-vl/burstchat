using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BurstChat.Shared.Schema.Servers
{
    /// <summary>
    /// This class represents a server.
    /// </summary>
    public class Server
    {
        /// <summary>
        ///  The id of the server.
        /// </summary>
        [Key]
        public int Id
        {
            get; set;
        }

        /// <summary>
        /// The name of the server.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// The date that the server was created.
        /// </summary>
        public DateTime DateCreated
        {
            get; set;
        }

        /// <summary>
        /// The channels available to the server.
        /// </summary>
        public List<Channel> Channels
        {
            get; set;
        }

        /// <summary>
        /// The list of subscribed users.
        /// </summary>
        public List<Subscription> Subscriptions
        {
            get; set;
        }
    }
}
