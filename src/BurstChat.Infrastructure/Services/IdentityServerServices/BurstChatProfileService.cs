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

namespace BurstChat.Infrastructure.Services.ProfileService;

public class BurstChatProfileService : IProfileService
{
    private readonly ILogger<BurstChatProfileService> _logger;
    private readonly IUserService _userService;

    public BurstChatProfileService(
        ILogger<BurstChatProfileService> logger,
        IUserService userService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

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
