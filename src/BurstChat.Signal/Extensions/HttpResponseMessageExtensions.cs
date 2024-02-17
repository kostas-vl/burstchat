using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using Newtonsoft.Json;

namespace BurstChat.Signal.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage httpResponse) =>
        JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());

    public static async Task<Either<TSuccess, Error>> ParseBurstChatApiResponseAsync<TSuccess>(this HttpResponseMessage httpResponse) =>
        httpResponse.StatusCode switch
        {
            HttpStatusCode.OK => new Success<TSuccess, Error>(await httpResponse.DeserializeContentAsync<TSuccess>()),
            HttpStatusCode.BadRequest => new Failure<TSuccess, Error>(await httpResponse.DeserializeContentAsync<Error>()),
            _ => new Failure<TSuccess, Error>(SystemErrors.Exception()),
        };

    public static async Task<Either<Unit, Error>> ParseBurstChatApiResponseAsync(this HttpResponseMessage httpResponse) =>
        httpResponse.StatusCode switch
        {
            HttpStatusCode.OK => new Success<Unit, Error>(new()),
            HttpStatusCode.BadRequest => new Failure<Unit, Error>(await httpResponse.DeserializeContentAsync<Error>()),
            _ => new Failure<Unit, Error>(SystemErrors.Exception()),
        };
}
