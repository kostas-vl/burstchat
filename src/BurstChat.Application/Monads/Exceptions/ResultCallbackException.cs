using System;

namespace BurstChat.Application.Monads;

public class ResultCallbackException : MonadException
{
    public ResultCallbackException(Exception inner)
        : base(
            ErrorLevel.Critical,
            ErrorType.System,
            "An error occured while executing result code, seem inner exception for details",
            inner
        ) { }
}
