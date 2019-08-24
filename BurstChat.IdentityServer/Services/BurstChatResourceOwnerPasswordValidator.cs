using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using BurstChat.IdentityServer.Services.UserService;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Users;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace BurstChat.IdentityServer.Services
{
    /// <summary>
    ///   This class is the BurstChat Api service implementation of the IResourceOwnerPasswordValidator interface.
    /// </summary>
    public class BurstChatResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<BurstChatResourceOwnerPasswordValidator> _logger;
        private readonly ISystemClock _clock;
        private readonly IUserService _userService;

        /// <summary>
        ///   Creates an instance of BurstChatResourceOwnerPasswordValidator.
        /// </summary>
        public BurstChatResourceOwnerPasswordValidator(
            ILogger<BurstChatResourceOwnerPasswordValidator> logger,
            ISystemClock clock,
            IUserService userService
        )
        {
            _logger = logger;
            _clock = clock;
            _userService = userService;
        }

        /// <summary>
        ///   Validates the resource owner password credential.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>A task instance</returns>
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var validationMonad = _userService
                .Validate(context.UserName, context.Password);

            var claimsMonad = validationMonad
                .Bind(_userService.GetClaims);

            if (claimsMonad is Success<IEnumerable<Claim>, Error> claims)
            {
                var user = (validationMonad as Success<User, Error>).Value;

                context.Result = new GrantValidationResult(user.Id.ToString(),
                                                           OidcConstants.AuthenticationMethods.Password,
                                                           _clock.UtcNow.UtcDateTime,
                                                           claims: claims.Value);
            }

            return Task.CompletedTask;
        }
    }
}
