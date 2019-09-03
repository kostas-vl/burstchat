using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace BurstChat.Signal.Services.HttpMessageHandlers
{
    /// <summary>
    ///   This class represents a middleware that is invoked when an http request is executed to the BurstChat API.
    ///   Its job is to assign the Authorization header with the proper access token.
    /// </summary>
    public class AuthorizationHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        ///   Creates a new instance of AuthorizationHeaderHandler.
        /// </summary>
        public AuthorizationHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        ///   The method that will executed when the middleware is invoked.
        /// </summary>
        /// <param name="request">The http request message instance</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous request</param>
        /// <returns>A task of HttpResponseMessage</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            StringValues authorizationHeader;
            try
            {
                authorizationHeader = _httpContextAccessor
                    .HttpContext
                    .Request
                    .Headers["Authorization"];
            }
            catch (Exception)
            {
                throw new Exception("Cant access authorization header");
            }

            try
            {
                if (!StringValues.IsNullOrEmpty(authorizationHeader))
                {
                    var accessToken = authorizationHeader.ToString();
                    request.Headers.Authorization = new AuthenticationHeaderValue(accessToken);
                }
            }
            catch (Exception)
            {
                throw new Exception("Cant assign the access token to the request");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
