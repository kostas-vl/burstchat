using System;
using System.Linq;
using BurstChat.Application.Monads;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Api.Extensions;

public static class HttpContextExtensions
{
    public static Result<long> GetUserId(this HttpContext context) =>
        context
            .Map(c => c.User.FindFirst("sub"))
            .Map(claim => Convert.ToInt64(claim?.Value))
            .MapErr(_ => AuthenticationException.Instance);

    public static string GetAccessToken(this HttpContext context)
    {
        try
        {
            var authorizationFound = context.Request.Headers.TryGetValue(
                "Authorization",
                out var bearerValue
            );

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
