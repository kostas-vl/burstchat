using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BurstChat.Shared.Schema.Chat;

namespace BurstChat.Shared.Schema.Servers
{
    /// <summary>
    /// This class represents a chat channel of a server.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// The id of the channel.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the channel.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A flag specifying whether all users of the server can access this channel.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// The date the channel was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The list of messages posted on the channel. In the context of entity framework
        /// this property is a single navigation property.
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
