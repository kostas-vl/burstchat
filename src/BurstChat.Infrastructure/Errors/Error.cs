using BurstChat.Application.Monads;

namespace BurstChat.Infrastructure.Errors;

public record Error(ErrorLevel Level, ErrorType Type, string Message);
