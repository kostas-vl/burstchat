using System;
using System.Text;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal.Extensions
{
    /// <summary>
    ///   This class contains static methods that extend an instance of the ILogger interface.
    /// </summary>
    public static class ILoggerExtensions
    {
        /// <summary>
        ///   This method will use the provided exception to log a new AspNet Core error based on
        ///   its content.
        /// </summary>
        /// <typeparam name="T">The type contained by the ILogger</typeparam>
        /// <param name="exception">The exception that will be logged</param>
        public static void LogException<T>(this ILogger<T> logger, Exception exception)
        {
            var builder = new StringBuilder();
            builder.AppendLine(exception.Message);
            builder.AppendLine(exception.StackTrace);

            var error = builder.ToString();

            logger.LogError(error);
        }

        /// <summary>
        ///   This method will log a new AspNet Core error based on the provided BurstChat Error.
        /// </summary>
        /// <typeparam name="T">The type contained by the ILogger</typeparam>
        /// <param name="error">The burst chat error instance</param>
        public static void LogBurstChatError<T>(this ILogger<T> logger, Error error)
        {
            var jsonError = JsonConvert.SerializeObject(error, Formatting.Indented);

            var builder = new StringBuilder();
            builder.AppendLine("BurstChat Error:");
            builder.AppendLine(jsonError);

            var logEntry = builder.ToString();

            logger.LogError(logEntry);
        }
    }
}
