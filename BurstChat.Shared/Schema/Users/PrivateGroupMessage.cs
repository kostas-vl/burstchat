using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Shared.Schema.Users
{
    /// <summary>
    /// This class represents a direct message sent from 2 users.
    /// </summary>
    public class PrivateGroupMessage
    {
        /// <summary>
        /// The id of the direct message.
        /// </summary>
        [Key]
        public long Id
        {
            get; set;
        }

        /// <summary>
        /// The name of the private group.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// The date the group was created.
        /// </summary>
        public DateTime DateCreated
        {
            get; set;
        }

        /// <summary>
        /// The list of users that are part of the private message group. This property in the context
        /// of entity framework is a single navigation property.
        /// </summary>
        public List<User> Users
        {
            get; set;
        }
        
        /// <summary>
        /// The list of message between the two participants. This property in the context
        /// of entity framework is a single navigation property.
        /// </summary>
        public List<Message> Messages
        {
            get; set;
        }
    }
}
