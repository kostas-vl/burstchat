using System;
using System.Threading.Tasks;

namespace BurstChat.Application.Monads
{
    /// <summary>
    ///   This class is a monad implementation of Either that can be used to represent a failed execution
    ///   of a method.
    /// </summary>
    /// <typeparam name="TSuccess">The type encapsulated by the success instance of the monad</typeparam>
    /// <typeparam name="TFailure">The type encapsulated by the failure instance of the monad</typeparam>
    public class Failure<TSuccess, TFailure> : Either<TSuccess, TFailure>
    {
        /// <summary>
        ///   The value contained within the monad.
        /// </summary>
        public TFailure Value
        {
            get;
        }

        /// <summary>
        ///   Executes any necessary start up code for the monad.
        /// </summary>
        public Failure(TFailure value)
        {
            Value = value;
        }

        /// <summary>
        ///   This method in a success scenario will invoke the callback provided and return its
        ///   results, and will not in a failure scenario.
        /// </summary>
        /// <typeparam name="TOut">The type encapsulated by the resulting Either monad</typeparam>
        /// <param name="callback">The callback to be executed</param>
        /// <returns>An either monad</returns>
        public override Either<TOut, TFailure> Bind<TOut>(Func<TSuccess, Either<TOut, TFailure>> callback)
        {
            return new Failure<TOut, TFailure>(Value);
        }

        /// <summary>
        ///   This method in a success scenario will invoke the callback asynchronously and return its
        ///   results. In a failure scenario it will not invoke the callback at all.
        /// </summary>
        /// <typeparam name="TOut">The type encapsulated by the resulting Either monad</typeparam>
        /// <param name="callbackTask">The callback task to be executed</param>
        /// <return>A task of an either monad</returns>
        public override Task<Either<TOut, TFailure>> BindAsync<TOut>(Func<TSuccess, Task<Either<TOut, TFailure>>> callback)
        {
            return Task.Run(() => new Failure<TOut, TFailure>(Value) as Either<TOut, TFailure>);
        }

        /// <summary>
        ///   This method will invoke the provided callback and will wrap the resulting instance
        ///   to an either monad and return it. This behavious is in a success scenario, the callback
        ///   will not execute in a failure scenario.
        /// </summary>
        /// <typeparam name="TOut">The type encapsulated by the resulting Either monad</typeparam>
        /// <param name="callback">The callback to be executed</param>
        /// <returns>An either monad</returns>
        public override Either<TOut, TFailure> Attach<TOut>(Func<TSuccess, TOut> callback)
        {
            return new Failure<TOut, TFailure>(Value);
        }

        /// <summary>
        ///   This method will invoke the provided callback asynchronously and will wrap the resulting instance
        ///   to an either monad and return it. This behavious is in a success scenario, the callback
        ///   will not execute in a failure scenario.
        /// </summary>
        /// <typeparam name="TOut">The type encapsulated by the resulting Either monad</typeparam>
        /// <param name="callback">The callback to be executed</param>
        /// <returns>A task of an either monad</returns>
        public override Task<Either<TOut, TFailure>> AttachAsync<TOut>(Func<TSuccess, Task<TOut>> callback)
        {
            return Task.Run(() => new Failure<TOut, TFailure>(Value) as Either<TOut, TFailure>);
        }

        /// <summary>
        ///   This method will execute the provided callback and simply return it self at the end.
        ///   This behaviour is the same in a success and in a failure scenario.
        /// </summary>
        /// <param name="callback">The callback to be executed</param>
        /// <returns>An either monad</returns>
        public override Either<TSuccess, TFailure> ExecuteAndContinue(Action<TSuccess> callback)
        {
            return this;
        }

        /// <summary>
        ///   This method will execute the provided callback asynchronously and simply return it self at the end.
        ///   This behaviour is the same in a success and in a failure scenario.
        /// </summary>
        /// <param name="callback">The callback to be executed</param>
        /// <returns>An either monad</returns>
        public override Task<Either<TSuccess, TFailure>> ExecuteAndContinueAsync(Func<TSuccess, Task> callback)
        {
            return Task.Run(() => this as Either<TSuccess, TFailure>);
        }
    }
}
