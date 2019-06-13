using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BurstChat.Shared.Schema.Servers;

namespace BurstChat.Shared.Schema.Users
{
    /// <summary>
    /// This class represents a user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The id of the user.
        /// </summary>
        [Key]
        public long Id
        {
            get; set;
        }

        /// <summary>
        /// The email of the user.
        /// </summary>        
        public string Email
        {
            get; set;
        }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password
        {
            get; set;
        }

        /// <summary>
        /// The date that the user was created.
        /// </summary>
        public DateTime DateCreated
        {
            get; set;
        }

        /// <summary>
        /// The list of subscribed servers.
        /// </summary>
        public List<Subscription> SubscribedServers
        {
            get; set;
        }
    }
}