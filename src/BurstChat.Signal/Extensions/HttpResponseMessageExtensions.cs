using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using Newtonsoft.Json;

namespace BurstChat.Signal.Extensions
{
    /// <summary>
    /// This class contains static methods that extend the HttpResponseMessage class.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// This method will read the resulting content of the http response and return an instance
        /// of the provided type.
        /// </summary>
        /// <typeparam name="T">The resulting type of the deserialized content</typeparam>
        /// <param name="httpResponse">The HttpResponseMessage instance</param>
        /// <returns>A task that encapsulates an instance of type T</returns>
        public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage httpResponse) =>
            JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());

        /// <summary>
        /// This method will return an either monad based on the content of the HttpResponseMessage instance.
        /// </summary>
        /// <typeparam name="TSuccess">The type that will be contained by the success monad</typeparam>
        /// <param name="httpResponse">The HttpResponseMessage instance</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public static async Task<Either<TSuccess, Error>> ParseBurstChatApiResponseAsync<TSuccess>(this HttpResponseMessage httpResponse) =>
            httpResponse.StatusCode switch
            {
                HttpStatusCode.OK => new Success<TSuccess, Error>(await httpResponse.DeserializeContentAsync<TSuccess>()),
                HttpStatusCode.BadRequest => new Failure<TSuccess, Error>(await httpResponse.DeserializeContentAsync<Error>()),
                _ => new Failure<TSuccess, Error>(SystemErrors.Exception()),
            };

        /// <summary>
        /// This method will return an either monad based on the content of the HttpResponseMessage instance.
        /// </summary>
        /// <param name="httpResponse">The HttpResponseMessage instance</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public static async Task<Either<Unit, Error>> ParseBurstChatApiResponseAsync(this HttpResponseMessage httpResponse) =>
            httpResponse.StatusCode switch
            {
                HttpStatusCode.OK => new Success<Unit, Error>(new()),
                HttpStatusCode.BadRequest => new Failure<Unit, Error>(await httpResponse.DeserializeContentAsync<Error>()),
                _ => new Failure<Unit, Error>(SystemErrors.Exception()),
            };
    }
}
