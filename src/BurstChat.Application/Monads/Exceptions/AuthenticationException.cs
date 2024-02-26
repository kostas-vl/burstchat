namespace BurstChat.Application.Monads;

public class AuthenticationException : MonadException
{
    public static AuthenticationException Instance = new();

    public AuthenticationException()
        : base(ErrorLevel.Critical, ErrorType.Validation, "The user was not found") { }
}
