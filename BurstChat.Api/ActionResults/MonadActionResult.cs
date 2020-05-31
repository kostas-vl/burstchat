using System;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.ActionResults
{
    public class MonadActionResult<TSuccess, TFailure> : ObjectResult
    {
        public MonadActionResult(object value) : base(value) { }

        public static implicit operator MonadActionResult<TSuccess, TFailure>(Either<TSuccess, TFailure> monad)
        {
            ObjectResult result = monad switch
            {
                Success<TSuccess, TFailure> s => new OkObjectResult(s.Value),

                Failure<TSuccess, TFailure> f when f.Value is AuthenticationError => new UnauthorizedObjectResult(f.Value),

                Failure<TSuccess, TFailure> f => new BadRequestObjectResult(f.Value),

                _ => new BadRequestObjectResult(SystemErrors.Exception())
            };

            return new MonadActionResult<TSuccess, TFailure>(result.Value)
            {
                ContentTypes = result.ContentTypes,
                DeclaredType = result.DeclaredType,
                Formatters = result.Formatters,
                StatusCode = result.StatusCode
            };
        }
    }
}

