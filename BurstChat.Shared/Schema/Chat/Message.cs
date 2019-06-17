using System;
using System.ComponentModel.DataAnnotations;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Shared.Schema.Chat
{
    /// <summary>
    /// This class represents a message sent by a user.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The id of the message.
        /// </summary>
        [Key]
        public long Id
        {
            get; set;
        }

        /// <summary>
        /// The id of the user that sent the message.
        /// </summary>
        public long UserId
        {
            get; set;
        }

        /// <summary>
        /// This is a reference navigation property of the user.
        /// </summary>
        public User User
        {
            get; set;
        }

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Content
        {
            get; set;
        }

        /// <summary>
        /// A flag specifying whether the message was edited by the user.
        /// </summary>
        public bool Edited
        {
            get; set;
        }

        /// <summary>
        /// The date that the message was posted.
        /// </summary>
        public DateTime DatePosted
        {
            get; set;
        }
    }
}
