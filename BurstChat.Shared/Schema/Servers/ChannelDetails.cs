using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BurstChat.Shared.Schema.Users;
using BurstChat.Shared.Schema.Chat;

namespace BurstChat.Shared.Schema.Servers
{
    /// <summary>
    /// This class represents a message of a chat channel.
    /// </summary>
    public class ChannelDetails
    {
        /// <summary>
        /// The id of the details entry.
        /// </summary>
        [Key]
        public int Id 
        {
            get; set;
        }
        
        /// <summary>
        /// The id of the channel to which the message was posted.
        /// </summary>
        public int ChannelId
        {
            get; set;
        }

        /// <summary>
        /// This is a reference navigation property of the channel.
        /// </summary>
        public Channel Channel 
        {
            get; set;
        }

        /// <summary>
        /// The list of users that are members of the channel. In the context of entity framework
        /// this property is a single navigation property.
        /// </summary>
        public List<User> Users
        {
            get; set;
        }

        /// <summary>
        /// The list of messages posted on the channel. In the context of entity framework
        /// this property is a single navigation property.
        public List<Message> Messages
        {
            get; set;
        }
    }
}
