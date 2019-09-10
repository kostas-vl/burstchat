using System;
using System.ComponentModel.DataAnnotations;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Shared.Schema.Servers
{
    /// <summary>
    ///     This class represents an assosiation between a server and an invitation sent to a user, for joining it.
    /// </summary>
    public class Invitation
    {
        /// <summary>
        ///     The identifier of the entry.
        /// </summary>
        [Key]
        public long Id 
        {
            get; set;
        }

        /// <summary>
        ///     The id of the server.
        /// </summary>
        public int ServerId
        {
            get; set;
        }

        /// <summary>
        ///   This is a reference navigation property of the server.
        /// </summary>
        public Server Server
        {
            get; set;
        }

        /// <summary>
        ///     The id of the target user.
        /// </summary>
        public long UserId
        {
            get;set;
        }

        /// <summary>
        ///   This is a reference navigation property of the server.
        /// </summary>
        public User User
        {
            get; set;
        }

        /// <summary>
        ///     A flag specifying the invitation was accepted.
        /// </summary>
        public bool Accepted
        {
            get; set;
        }

        /// <summary>
        ///     A flag specifying the invitation was declined.
        /// </summary>
        public bool Declined
        {
            get; set;
        }

        /// <summary>
        ///     The date that the entry updated either the Accepted or Declined properties.
        /// </summary>
        public DateTime? DateUpdated
        {
            get; set;
        }

        /// <summary>
        ///     The date that the entry was created.
        /// </summary>
        public DateTime DateCreated
        {
            get; set;
        }
    }
}