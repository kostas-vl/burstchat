namespace BurstChat.Application.Errors;

public class AuthenticationError : Error
{
    public AuthenticationError()
        : base(ErrorLevel.Critical, ErrorType.Validation, "The user was not found") { }
}
