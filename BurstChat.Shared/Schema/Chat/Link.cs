using System;
using System.ComponentModel.DataAnnotations;

namespace BurstChat.Shared.Schema.Chat
{
    /// <summary>
    ///     This class stores urls posted on messages.
    /// </summary>
    public class Link
    {
        /// <summary>
        ///     An identifier for the url.
        /// </summary>
        [Key]
        public long Id
        {
            get; set;
        }

        /// <summary>
        ///     The url posted.
        /// </summary>
        public string Url
        {
            get; set;
        }

        /// <summary>
        ///     The date the entry was created.
        /// </summary>
        public DateTime DateCreated
        {
            get; set;
        }
    }
}