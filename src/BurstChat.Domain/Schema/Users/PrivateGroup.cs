using System;
using System.Collections.Generic;
using BurstChat.Domain.Schema.Chat;

namespace BurstChat.Domain.Schema.Users
{
    /// <summary>
    /// This class represents a private group message sent from multiple users.
    /// </summary>
    public class PrivateGroup
    {
        /// <summary>
        /// The id of the private group message.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The name of the private group.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The date the group was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The list of users that are part of the private message group. This property in the context
        /// of entity framework is a single navigation property.
        /// </summary>
        public List<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// The list of message between the two participants. This property in the context
        /// of entity framework is a single navigation property.
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
