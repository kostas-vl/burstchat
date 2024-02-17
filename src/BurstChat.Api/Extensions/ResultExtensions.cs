using System.Threading.Tasks;
using BurstChat.Application.Monads;
using BurstChat.Application.Errors;
using Microsoft.AspNetCore.Mvc;

namespace BurstChat.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult Into<T>(this Result<T> result)
    {
        return result switch
        {
            Ok<T> { Value: var value } => new OkObjectResult(value),

            Err<T> { Value: AuthenticationException err } => new UnauthorizedObjectResult(err),

            Err<T> { Value: var err } => new BadRequestObjectResult(err),

            _ => new BadRequestObjectResult(SystemErrors.Exception),
        };
    }

    public static async Task<IActionResult> IntoAsync<T>(this Task<Result<T>> result)
    {
        return (await result).Into();
    }
}
