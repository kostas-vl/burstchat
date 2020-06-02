using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using Newtonsoft.Json;

namespace BurstChat.Signal.Extensions
{
    /// <summary>
    ///   This class contains static methods that extend the HttpResponseMessage class.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        ///   This method will read the resulting content of the http response and return an instance
        ///   of the provided type.
        /// </summary>
        /// <typeparam name="T">The resulting type of the deserialized content</typeparam>
        /// <param name="httpResponse">The HttpResponseMessage instance</param>
        /// <returns>A task that encapsulates an instance of type T</returns>
        public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage httpResponse)
        {
            var content = await httpResponse
                .Content
                .ReadAsStringAsync();

            var deserializedContent = JsonConvert.DeserializeObject<T>(content);

            return deserializedContent;
        }

        /// <summary>
        ///   This method will return an either monad based on the content of the HttpResponseMessage instance.
        /// </summary>
        /// <typeparam name="TSuccess">The type that will be contained by the success monad</typeparam>
        /// <param name="httpResponse">The HttpResponseMessage instance</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public static async Task<Either<TSuccess, Error>> ParseBurstChatApiResponseAsync<TSuccess>(this HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    var success = await httpResponse.DeserializeContentAsync<TSuccess>();
                    return new Success<TSuccess, Error>(success);

                case HttpStatusCode.BadRequest:
                    var failure = await httpResponse.DeserializeContentAsync<Error>();
                    return new Failure<TSuccess, Error>(failure);

                default:
                    return new Failure<TSuccess, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will return an either monad based on the content of the HttpResponseMessage instance.
        /// </summary>
        /// <param name="httpResponse">The HttpResponseMessage instance</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public static async Task<Either<Unit, Error>> ParseBurstChatApiResponseAsync(this HttpResponseMessage httpResponse)
        {
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new Success<Unit, Error>(new Unit());

                case HttpStatusCode.BadRequest:
                    var failure = await httpResponse.DeserializeContentAsync<Error>();
                    return new Failure<Unit, Error>(failure);

                default:
                    return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }
    }
}
