using System;

namespace BurstChat.Domain.Schema.Chat
{
    /// <summary>
    /// This class stores urls posted on messages.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// An identifier for the url.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The url posted.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// The date the entry was created.
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
