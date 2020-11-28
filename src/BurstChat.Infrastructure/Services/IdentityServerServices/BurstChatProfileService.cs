using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.UserService;
using BurstChat.Domain.Schema.Users;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace BurstChat.Infrastructure.Services.ProfileService
{
    /// <summary>
    /// The BurstChat implementation of the Identity Server 4 IProfileService interface.
    /// </summary>
    public class BurstChatProfileService : IProfileService
    {
        private readonly ILogger<BurstChatProfileService> _logger;
        private readonly IUserService _userService;

        /// <summary>
        /// Creates an instance of BurstChatProfileService.
        /// 
        /// Exceptions:
        ///     ArgumentNullException: When any parameter is null.
        /// </summary>
        public BurstChatProfileService(
            ILogger<BurstChatProfileService> logger,
            IUserService userService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A task instance</returns>
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(_logger);

            if (context.RequestedClaimTypes.Any())
            {
                var subjectId = context
                    .Subject
                    .GetSubjectId();
                var userId = Convert.ToInt64(subjectId);
                var userMonad = _userService.Get(userId);
                var claimsMonad = userMonad.Bind(_userService.GetClaims);

                if (claimsMonad is Success<IEnumerable<Claim>, Error> claims)
                    context.AddRequestedClaims(new Claim[0]);
            }

            context.LogIssuedClaims(_logger);

            return Task.CompletedTask;
        }

        /// <summary>
        /// This method gets called whenecet identity server needs to determine if teh user is valid or active (e.g. if the user's account has been deactivated since they lodded in).
        /// (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A task instance</returns>
        public Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);

            var subjectId = context
                .Subject
                .GetSubjectId();
            var userId = Convert.ToInt64(subjectId);
            var userMonad = _userService.Get(userId);

            context.IsActive = userMonad is Success<User, Error>;

            return Task.CompletedTask;
        }
    }
}
