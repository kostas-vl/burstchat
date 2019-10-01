using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BurstChat.Shared.Schema.Chat;

namespace BurstChat.Shared.Schema.Users
{
    /// <summary>
    ///     This class represents direct messages sent between two users.
    /// </summary>
    public class DirectMessaging
    {
        /// <summary>
        ///     The id of the direct messaging.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     The user id of the first participant.
        /// </summary>
        public long FirstParticipantUserId { get; set; }

        /// <summary>
        ///     The user id of the second participant.
        /// </summary>
        public long SecondParticipantUserId { get; set; }

        /// <summary>
        ///     The list of messages sent between the users.
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();       
    }
}