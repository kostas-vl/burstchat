using System;
using BurstChat.Shared.Context;
using BurstChat.Shared.Schema.Users;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Services.UserService
{
    /// <summary>
    /// This class is the base implementation of the IUserService.
    /// </summary>
    public class UserProvider : IUserService
    {
        private readonly ILogger<UserProvider> _logger;
        private readonly BurstChatContext _burstChatContext;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// </summary>
        public UserProvider(
            ILogger<UserProvider> logger,
            BurstChatContext burstChatContext
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
        }

        /// <summary>
        /// This method return a User instance based on the id provided.
        /// </summary>
        public User Get(long id)
        {
            return null;
        }
    }
}
