using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace BurstChat.Infrastructure.Extensions
{
    /// <summary>
    /// This class contains static methods that extend the HttpResponseMessage class.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// This method will read the resulting content of the http response and return
        /// an instance of the provided type.
        /// </summary>
        /// <typeparam name="T">The resulting type of the deserialized content</typeparam>
        /// <param name="response">the HttpResponseMessage instance</param>
        /// <returns>A task that encapsulated an instance of type T</returns>
        public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage response)
        {
            var content = await response
                .Content
                .ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(content);
        }
    }
}
