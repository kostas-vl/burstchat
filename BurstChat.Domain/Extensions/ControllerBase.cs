using System;
using BurstChat.Domain.Errors;
using BurstChat.Domain.Monads;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Domain.Extensions
{
    /// <summary>
    ///   This class contains static methods that extends the functionality of a ControllerBase instance.
    /// </summary>
    public static class ControllerBaseExtensions
    {
        /// <summary>
        ///   This method will unwrap a provided monad in order to return a proper IActionResult instance.
        ///
        ///   Exceptions
        ///     NotImplementedException: When the provided monad is not of type Success<TSuccess, TFailure> or Failure<TSuccess, TFailure>.
        /// </summary>
        /// <param name="controller">The ControllerBase instance that is extended</param>
        /// <param name="monad">The monad to be unwraped</param>
        /// <returns>An IActionResult instance</returns>
        public static IActionResult UnwrapMonad<TSuccess, TFailure>(this ControllerBase controller, Either<TSuccess, TFailure> monad) =>
            monad switch
            {
                Success<TSuccess, TFailure> s => controller.Ok(s.Value),

                Failure<TSuccess, TFailure> f when f.Value is AuthenticationError => controller.Unauthorized(),

                Failure<TSuccess, TFailure> f => controller.BadRequest(f.Value),

                _ => controller.BadRequest(SystemErrors.Exception())
            };

        /// <summary>
        ///   This method will unwrap a provided monad in order to return a proper IActionResult instance.
        ///   Significant is that this method supports Success monads that will return an empty Ok IActionResult.
        ///
        ///   Exceptions
        ///     NotImplementedException: When the provided monad is not of type Success<Unit, TFailure> or Failure<Unit, TFailure>.
        /// </summary>
        /// <param name="controller">The ControllerBase instance that is extended</param>
        /// <param name="monad">The monad to be unwraped</param>
        /// <returns>An IActionResult instance</returns>
        public static IActionResult UnwrapMonad<TFailure>(this ControllerBase controller, Either<Unit, TFailure> monad) =>
            monad switch
            {
                Success<Unit, TFailure> _ => controller.Ok(),

                Failure<Unit, TFailure> f when f.Value is AuthenticationError => controller.Unauthorized(),

                Failure<Unit, TFailure> f => controller.BadRequest(f.Value),

                _ => controller.BadRequest(SystemErrors.Exception())
            };
    }
}
