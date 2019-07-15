using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Extensions
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
        /// <param name="logger">The logger instance of which the method is the extension</param>
        /// <param name="exception">The exception instance to be logged</param>
        public static void LogException<T>(this ILogger<T> logger, Exception exception)
        {
            if (exception != null) 
            {
                var builder = new StringBuilder();
                builder.AppendLine(exception.Message);
                builder.AppendLine(exception.StackTrace);

                var error = builder.ToString();

                logger.LogError(error);
            }
        }
    }
}
