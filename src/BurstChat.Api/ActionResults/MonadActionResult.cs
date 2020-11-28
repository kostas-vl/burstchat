using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.ActionResults
{
    /// <summary>
    /// A class that can represent the instance of an Either monad as an ActionResult.
    /// </summary>
    public class MonadActionResult<TSuccess, TFailure> : ObjectResult
    {
        /// <summary>
        /// Creates a new instance of MonadActionResult.
        /// </summary>
        public MonadActionResult(object value) : base(value) { }

        /// <summary>
        /// An operator that creates a new instance of MonadActionResult from the instance
        /// of an Either monad.
        /// </summary>
        /// <param name="monad">The either monad instance</param>
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

