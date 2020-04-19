using System;
using System.ComponentModel.DataAnnotations;

namespace BurstChat.Shared.Schema.Alpha
{
    /// <summary>
    /// This class represents information about an alpha invitation code that enables a user
    /// to create an account on BurstChat.
    /// </summary>
    public class AlphaInvitation
    {
        /// <summary>
        /// The id of the entry.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// The invitation code.
        /// </summary>
        public Guid Code { get; set; }

        /// <summary>
        /// The date the entry was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The date the invitation code expires.
        /// </summary>
        public DateTime DateExpired { get; set; }
    }
}
