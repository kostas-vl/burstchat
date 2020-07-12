using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;

namespace BurstChat.Domain.Schema.Users
{
    /// <summary>
    /// This class represents a user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The id of the user.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The password of the user.
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The user's avatar binary data.
        /// </summary>
        public byte[] Avatar { get; set; } = new byte[0];

        /// <summary>
        /// The date that the user was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The list of subscribed servers.
        /// </summary>
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        /// <summary>
        ///   The list of one time passwords issued by the user.
        /// </summary>
        [JsonIgnore]
        public List<OneTimePassword> OneTimePasswords { get; set; } = new List<OneTimePassword>();
    }
}
