namespace BurstChat.Application.Errors
{
    /// <summary>
    /// This class is designed to inform the execution of the program that the result of an operation
    /// was not successful. This type of improper execution is associated with the design of an operation
    /// and its not language level error, although it can represent one.
    /// </summary>
    public record Error(ErrorLevel level, ErrorType Type, string Message);

    // public class Error
    // {
    //     /// <summary>
    //     /// The level of the error.
    //     /// </summary>
    //     public string Level
    //     {
    //         get;
    //     }

    //     /// <summary>
    //     /// The type of the error in the context of the program's execution.
    //     /// </summary>
    //     public string Type
    //     {
    //         get;
    //     }

    //     /// <summary>
    //     /// The descriptive message of the error.
    //     /// </summary>
    //     public string Message
    //     {
    //         get;
    //     }

    //     /// <summary>
    //     /// Makes all the necessary assignements for the proper construction
    //     /// of the error instance.
    //     /// </summary>
    //     public Error(ErrorLevel level, ErrorType type, string message)
    //     {
    //         Level = level.ToString();
    //         Type = type.ToString();
    //         Message = message;
    //     }
    // }
}
