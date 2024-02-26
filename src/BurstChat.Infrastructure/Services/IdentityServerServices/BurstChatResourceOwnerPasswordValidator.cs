using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.UserService;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace BurstChat.Infrastructure.Services.ResourceOwnerPasswordValidator;

public class BurstChatResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly ILogger<BurstChatResourceOwnerPasswordValidator> _logger;
    private readonly ISystemClock _clock;
    private readonly IUserService _userService;

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

    public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var validationResult = _userService.Validate(context.UserName, context.Password);
        var claimsResult = validationResult.And(_userService.GetClaims);

        if (claimsResult.IsOk)
        {
            var user = validationResult.Unwrap();
            var claims = claimsResult.Unwrap();
            context.Result = new GrantValidationResult(
                user.Id.ToString(),
                OidcConstants.AuthenticationMethods.Password,
                _clock.UtcNow.UtcDateTime,
                claims
            );
        }
        else
        {
            var error = UserErrors.UserPasswordDidNotMatch;
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                error.Message
            );
        }

        return Task.CompletedTask;
    }
}
