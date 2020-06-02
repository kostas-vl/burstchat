using System;

namespace BurstChat.Application.Monads
{
    /// <summary>
    /// This class represents that a monad does not return a value of any importance.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// Creates a new instance of Unit.
        /// </summary>
        public static Unit New() => new Unit();
    }
}
