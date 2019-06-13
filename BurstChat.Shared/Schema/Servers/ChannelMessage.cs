using System;
using System.ComponentModel.DataAnnotations;

namespace BurstChat.Shared.Schema.Servers
{
    /// <summary>
    /// This class represents a message of a chat channel.
    /// </summary>
    public class ChannelMessage
    {
        /// <summary>
        ///  The id of the message.
        /// </summary>
        [Key]
        public long Id
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
        /// The id of the user that posted the message.
        /// </summary>
        public long UserId
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
        /// A flag specifying if the message was edited.
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