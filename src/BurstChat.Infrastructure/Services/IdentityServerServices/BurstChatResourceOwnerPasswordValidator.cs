using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.UserService;
using BurstChat.Domain.Schema.Users;
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
        var validationMonad = _userService
            .Validate(context.UserName, context.Password);

        var claimsMonad = validationMonad
            .Bind(_userService.GetClaims);

        switch (claimsMonad)
        {
            case Success<IEnumerable<Claim>, Error> claims:
                var user = (validationMonad as Success<User, Error>)!.Value;
                context.Result = new GrantValidationResult(user.Id.ToString(),
                                                           OidcConstants.AuthenticationMethods.Password,
                                                           _clock.UtcNow.UtcDateTime,
                                                           claims: claims.Value);
                break;

            case Failure<IEnumerable<Claim>, Error> _:
                var error = UserErrors.UserPasswordDidNotMatch();
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, error.Message);
                break;

            default:
                break;

        }

        return Task.CompletedTask;
    }
}
