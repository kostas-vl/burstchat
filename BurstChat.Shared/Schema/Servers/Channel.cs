using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public int Id
        {
            get; set;
        }

        /// <summary>
        /// The id of the server that the channel belongs.
        /// </summary>
        public int ServerId
        {
            get; set;
        }

        /// <summary>
        /// The name of the channel.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// A flag specifying whether all users of the server can access this channel.
        /// </summary>
        public bool IsPublic
        {
            get; set;
        }

        /// <summary>
        /// The date the channel was created.
        /// </summary>
        public DateTime DateCreated
        {
            get; set;
        }

        /// <summary>
        /// All messages posted to the channel.
        /// </summary>
        public List<ChannelMessage> Messages
        {
            get; set;
        }
    }
}