using BurstChat.Application.Monads;
using BurstChat.Infrastructure.Errors;

namespace BurstChat.Infrastructure.Extensions;

public static class MonadExceptionExtensions
{
    public static Error Into(this MonadException ex) => new(ex.Level, ex.Type, ex.Message);
}
