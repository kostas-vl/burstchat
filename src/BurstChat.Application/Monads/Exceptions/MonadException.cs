using System;

namespace BurstChat.Application.Monads;

public class MonadException : Exception
{
    public ErrorLevel Level { get; }

    public ErrorType Type { get; }

    public MonadException(ErrorLevel level, ErrorType type, string message)
        : base(message)
    {
        Level = level;
        Type = type;
    }

    public MonadException(ErrorLevel level, ErrorType type, string message, Exception inner)
        : base(message, inner)
    {
        Level = level;
        Type = type;
    }
}
