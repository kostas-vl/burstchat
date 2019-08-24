using System;
using System.Text;
using BurstChat.Shared.Errors;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace BurstChat.IdentityServer.Extensions
{
    /// <summary>
    /// This static class contains a collection of static methods for the ILogger interface.
    /// </summary>
    public static class ILoggerExtensions
    {
        /// <summary>
        /// This static method uses the ILogger interface of AspNet core in order
        /// to log the provided exception as an error.
        /// This method will do nothing if the exception instance is null.
        /// </summary>
        /// <typeparam name="T">The type encapsulated by the ILogger instance</typeparam>
        /// <param name="logger">The logger instance of which the method is the extension</param>
        /// <param name="exception">The exception instance to be logged</param>
        public static void LogException<T>(this ILogger<T> logger, Exception exception)
        {
            if (exception != null) 
            {
                var builder = new StringBuilder();
                builder.AppendLine(exception.Message);
                builder.AppendLine(exception.StackTrace);

                var logEntry = builder.ToString();

                logger.LogError(logEntry);
            }
        }

        /// <summary>
        ///   This static method uses the ILogger interface of AspNet core in order
        ///   to log the provided BurstChat error as an ILogger Error entry.
        ///   This method will do nothing if the BurstChat error instance is null.
        /// </summary>
        /// <typeparam name="T">The type encapsulated by the ILogger instance</typeparam>
        /// <param name="logger">The logger instance of which the method is extending</param>
        /// <param name="error">The BurstChat error instance to be logged</param>
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
