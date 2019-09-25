using System;

namespace BurstChat.Shared.Schema.Users
{
    /// <summary>
    ///   This class represents an one time password that a user issued.
    /// </summary>
    public class OneTimePassword
    {
        /// <summary>
        ///   The id of the one time password entry.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///   The one time password hashed value.
        /// </summary>
        public string OTP { get; set; } = string.Empty;

        /// <summary>
        ///   The date the one time password was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///   The date that the one time password expires.
        /// </summary>
        public DateTime ExpirationDate { get; set; }
    }
}
