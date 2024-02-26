using System.Text.Json.Serialization;

namespace BurstChat.Application.Errors
{
    /// <summary>
    /// This class is designed to inform the execution of the program that the result of an operation
    /// was not successful. This type of improper execution is associated with the design of an operation
    /// and its not language level error, although it can represent one.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// The level of the error.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ErrorLevel Level { get; }

        /// <summary>
        /// The type of the error in the context of the program's execution.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ErrorType Type { get; }

        /// <summary>
        /// The descriptive message of the error.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public string Message { get; }

        /// <summary>
        /// Makes all the necessary assignements for the proper construction
        /// of the error instance.
        /// </summary>
        public Error(ErrorLevel level, ErrorType type, string message)
        {
            Level = level;
            Type = type;
            Message = message;
        }
    }
}
