using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Signal.Extensions;
using BurstChat.Signal.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BurstChat.Signal.Services.ApiInteropService
{
    public class BurstChatApiInteropService
    {
        private readonly ApiDomainOptions _acceptedDomainsOptions;
        private readonly HttpClient _httpClient;

        /// <summary>
        ///     Creates a new instance of BurstChatApiInteropService.
        /// </summary>
        public BurstChatApiInteropService(
            IOptions<ApiDomainOptions> acceptedDomainsOptions,
            HttpClient httpClient
        )
        {
            _acceptedDomainsOptions = acceptedDomainsOptions.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_acceptedDomainsOptions.BurstChatApiDomain);
        }

        /// <summary>
        ///     Sends a request to the BurstChat API based on the provided parameters and returns an either monad containing
        ///     the specified type or an error.
        /// </summary>
        /// <typeparam name="T">The type contained by a success monad</typeparam>
        /// <param name="method">The http method of the request</param>
        /// <param name="path">The path of the request</param>
        /// <param name="content">Optional content for the request</param>
        /// <returns>A task of an either monad</returns>
        public async Task<Either<T, Error>> SendAsync<T>(HttpContext context, HttpMethod method, string path, HttpContent? content = null)
        {
            var accessToken = context.GetAccessToken();
            var request = new HttpRequestMessage(method, path);
            request.Content = content;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            return await response.ParseBurstChatApiResponseAsync<T>();
        }

        /// <summary>
        ///     Sends a request to the BurstChat API based on the provided parameters and returns an either monad containing
        ///     a unit instance or an error.
        /// </summary>
        /// <param name="method">The http method of the request</param>
        /// <param name="path">The path of the request</param>
        /// <param name="content">Optional content for the request</param>
        /// <returns>A task of an either monad</returns>
        public async Task<Either<Unit, Error>> SendAsync(HttpContext context, HttpMethod method, string path, HttpContent? content = null)
        {
            var accessToken = context.GetAccessToken();
            var request = new HttpRequestMessage(method, path);
            request.Content = content;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            return await response.ParseBurstChatApiResponseAsync();
        }
    }
}
