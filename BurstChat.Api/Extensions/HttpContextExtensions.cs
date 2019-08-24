using System;
using System.Security.Claims;
using BurstChat.Api.Errors;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using Microsoft.AspNetCore.Http;

namespace BurstChat.Api.Extensions
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
                var subject = context
                    .User
                    .FindFirst("sub");

                var userId = Convert.ToInt64(subject);

                return new Success<long, Error>(userId);
            }
            catch 
            {
                return new Failure<long, Error>(UserErrors.UserNotFound());
            }
        }
    }
}
