using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Shared.Extensions
{
    /// <summary>
    ///   This class contains extensions methods for an HttpContext instance.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        ///   This method will get the user id for the current HttpContext instance based on the subject id
        ///   of the authenticated user's claims.
        /// </summary>
        /// <param name="context">The http context instance</param>
        /// <returns>An either monad</returns>
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

        /// <summary>
        ///   This method will fetch the value of a Bearer access token based on the instance of the HttpContext.
        /// </summary>
        /// <param name="context">The http context instance</param>
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
}
