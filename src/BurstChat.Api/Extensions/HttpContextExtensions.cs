using System;
using System.Linq;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Api.Extensions;

public static class HttpContextExtensions
{
    public static Either<long, Error> GetUserId(this HttpContext context)
    {
        try
        {
            var subjectClaim = context
                .User
                .FindFirst("sub");

            if (subjectClaim is { })
            {
                var userId = Convert.ToInt64(subjectClaim.Value);
                return new Success<long, Error>(userId);
            }
            else
                return new Failure<long, Error>(new AuthenticationError());
        }
        catch
        {
            return new Failure<long, Error>(new AuthenticationError());
        }
    }

    public static string GetAccessToken(this HttpContext context)
    {
        try
        {
            var authorizationFound = context
                .Request
                .Headers
                .TryGetValue("Authorization", out var bearerValue);

            if (authorizationFound)
            {
                var plainValue = bearerValue.ToString();
                return plainValue.Split("Bearer ").Last();
            }

            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
