namespace BurstChat.Application.Errors;

public record AuthenticationError : Error
{
    public AuthenticationError() : base(ErrorLevel.Critical,
                                        ErrorType.Validation,
                                        "The user was not found") { }
}
