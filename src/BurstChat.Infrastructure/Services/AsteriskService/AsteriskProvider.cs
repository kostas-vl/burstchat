using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Models;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace BurstChat.Infrastructure.Services.AsteriskService
{
    /// <summary>
    /// This class is the base implementation of the IAsteriskService interface.
    /// </summary>
    public class AsteriskProvider : IAsteriskService
    {
        private readonly string _pjsipPath = "/ari/asterisk/config/dynamic/res_pjsip";
        private readonly ILogger<AsteriskProvider> _logger;
        private readonly AsteriskOptions _options;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Creates a new instance of AsteriskProvider.
        ///
        /// Exceptions
        ///     ArgumentNullException: when any parameter is null.
        /// </summary>
        public AsteriskProvider(
            ILogger<AsteriskProvider> logger,
            IOptions<AsteriskOptions> options,
            HttpClient httpClient
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Executes a request to an Asterisk server in order to create a new aor configuration
        /// for the provided endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint name</param>
        /// <returns>A task of an either monad</returns>
        private async Task<Either<Unit, Error>> PostAorAsync(Guid endpoint)
        {
            try
            {
                var path = $"{_options.Domain}{_pjsipPath}/aor/aor{endpoint}";
                var request = new HttpRequestMessage(HttpMethod.Put, path);
                var content = new AsteriskFields
                {
                    Fields = new List<AsteriskProperty>
                    {
                        new AsteriskProperty { Attribute = "id", Value = $"aor{endpoint}" },
                        new AsteriskProperty { Attribute = "max_contacts", Value = "5" },
                    }
                };
                var json = JsonSerializer.Serialize(content);

                request.Content  = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new BasicAuthenticationHeaderValue(_options.Username, _options.Password);

                var response = await _httpClient.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new Success<Unit, Error>(Unit.New());

                    case HttpStatusCode.BadRequest:
                        var error = await response.Content.ReadAsStringAsync();
                        _logger.LogError(error);
                        return new Failure<Unit, Error>(SystemErrors.Exception());

                    default:
                        return new Failure<Unit, Error>(SystemErrors.Exception());
                }
            } catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Sends an Http request to a remote Asterisk server in order to create a new
        /// auth configuration for the provided pjsip enpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint name</param>
        /// <param name="password">The endpoint password</param>
        /// <returns>A task of an either monad</returns>
        private async Task<Either<Unit, Error>> PostAuthAsync(Guid endpoint, Guid password)
        {
            try
            {
                var path = $"{_options.Domain}{_pjsipPath}/auth/auth{endpoint}";
                var request = new HttpRequestMessage(HttpMethod.Put, path);
                var content = new AsteriskFields
                {
                    Fields = new List<AsteriskProperty>
                    {
                        new AsteriskProperty { Attribute = "id", Value = $"auth{endpoint}" },
                        new AsteriskProperty { Attribute = "auth_type", Value = "userpass" },
                        new AsteriskProperty { Attribute = "username", Value = endpoint.ToString() },
                        new AsteriskProperty { Attribute = "password", Value = password.ToString() }
                    }
                };
                var json = JsonSerializer.Serialize(content);

                request.Content  = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new BasicAuthenticationHeaderValue(_options.Username, _options.Password);

                var response = await _httpClient.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new Success<Unit, Error>(Unit.New());

                    case HttpStatusCode.BadRequest:
                        var error = await response.Content.ReadAsStringAsync();
                        _logger.LogError(error);
                        return new Failure<Unit, Error>(SystemErrors.Exception());

                    default:
                        return new Failure<Unit, Error>(SystemErrors.Exception());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Sends an Http request to a remote Asterisk server in order to create a new
        /// pjsip endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint name</param>
        /// <returns>A task of an either monad</returns>
        private async Task<Either<Unit, Error>> PostEndpointAsync(Guid endpoint)
        {
            try
            {
                var path = $"{_options.Domain}{_pjsipPath}/endpoint/{endpoint}";
                var request = new HttpRequestMessage(HttpMethod.Put, path);
                var content = new AsteriskFields
                {
                    Fields = new List<AsteriskProperty>
                    {
                        new AsteriskProperty { Attribute = "id", Value = endpoint.ToString() },
                        new AsteriskProperty { Attribute = "transport", Value = "transport-ws" },
                        new AsteriskProperty { Attribute = "aors", Value = $"aor{endpoint}" },
                        new AsteriskProperty { Attribute = "auth", Value = $"auth{endpoint}" },
                        new AsteriskProperty { Attribute = "context", Value = "burst" },
                        new AsteriskProperty { Attribute = "disallow", Value = "all" },
                        new AsteriskProperty { Attribute = "allow", Value = "opus" },
                        new AsteriskProperty { Attribute = "dtls_auto_generate_url", Value = "yes" }
                    }
                };
                var json = JsonSerializer.Serialize(content);

                request.Content  = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new BasicAuthenticationHeaderValue(_options.Username, _options.Password);

                var response = await _httpClient.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return new Success<Unit, Error>(Unit.New());

                    case HttpStatusCode.BadRequest:
                        var error = await response.Content.ReadAsStringAsync();
                        _logger.LogError(error);
                        return new Failure<Unit, Error>(SystemErrors.Exception());

                    default:
                        return new Failure<Unit, Error>(SystemErrors.Exception());
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Sends an Http request to a remore Asterisk server and returns information
        /// about a pjsip endpoint, if that exists.
        /// </summary>
        /// <param name="endpoint">The endpoint name</param>
        /// <returns>A task of an either monad</returns>
        public async Task<Either<AsteriskEndpoint, Error>> GetAsync(Guid endpoint)
        {
            try
            {
                var path = $"{_options.Domain}{_pjsipPath}/endpoint/{endpoint}";
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Authorization = new BasicAuthenticationHeaderValue(_options.Username, _options.Password);

                var response = await _httpClient.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var properties = await response.DeserializeContentAsync<List<AsteriskProperty>>();
                        var password = properties.First(p => p.Attribute == "password");
                        var info = new AsteriskEndpoint
                        {
                            Username = endpoint.ToString(),
                            Password = password.Value
                        };
                        return new Success<AsteriskEndpoint, Error>(info);

                    case HttpStatusCode.BadRequest:
                        var error = await response.Content.ReadAsStringAsync();
                        _logger.LogError(error);
                        return new Failure<AsteriskEndpoint, Error>(SystemErrors.Exception());

                    default:
                        return new Failure<AsteriskEndpoint, Error>(SystemErrors.Exception());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<AsteriskEndpoint, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Sends an Http request to a remote Asterisk server in order to create a new
        /// pjsip aor, auth and endpoint. If the operations are all successful an instance
        /// of AsteriskEndpoint is returned.
        /// </summary>
        /// <param name="endpoint">The endpoint name</param>
        /// <param name="password">The password for the endpoint</param>
        /// <returns>A task of an either monad</returns>
        public async Task<Either<AsteriskEndpoint, Error>> PostAsync(Guid endpoint, Guid password)
        {
            var aorMonad = await PostAorAsync(endpoint);
            var authMonad = await aorMonad.BindAsync(_ => PostAuthAsync(endpoint, password));
            var endpointMonad = await authMonad.BindAsync(_ => PostEndpointAsync(endpoint));

            return endpointMonad.Attach(_ => new AsteriskEndpoint
            {
                Username = endpoint.ToString(),
                Password = password.ToString()
            });
        }
    }
}
