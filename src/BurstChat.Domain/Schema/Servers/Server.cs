using System;
using System.Collections.Generic;

namespace BurstChat.Domain.Schema.Servers
{
    /// <summary>
    /// This class represents a server.
    /// </summary>
    public class Server
    {
        /// <summary>
        ///  The id of the server.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the server.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The server's avatat, binary data.
        /// </summary>
        public byte[] Avatar { get; set; } = new byte[0];

        /// <summary>
        /// The date that the server was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The channels available to the server.
        /// </summary>
        public List<Channel> Channels { get; set; } = new List<Channel>();

        /// <summary>
        /// The list of subscribed users.
        /// </summary>
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
