using System;

namespace BurstChat.Signal.Models
{
    /// <summary>
    ///     The base response from the Signal server to specified clients.
    /// </summary>
    /// <typeparam name="T">The type contained by the payload</typeparam>
    public class Payload<T>
    {
        /// <summary>
        ///     The name of the signal group.
        /// </summary>
        public string SignalGroup
        {
            get; set;
        }

        /// <summary>
        ///     The content of the payload response.
        /// </summary>
        public T Content
        {
            get; set;
        }
    }
}