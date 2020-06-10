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
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace BurstChat.Infrastructure.Services.AsteriskService
{
    /// <summary>
    /// This class is the base implementation of the IAsteriskService interface.
    /// </summary>
    public class AsteriskProvider : IAsteriskService, IDisposable
    {
        private readonly ILogger<AsteriskProvider> _logger;
        private readonly DatabaseOptions _options;
        private NpgsqlConnection _connection;

        private readonly string InsertAor = @"
            insert into ps_aors
                (id, max_contacts, remove_existing, support_path)
            values
                (@id, @maxContacts, @removeExisting, @supportPath);
        ";

        private readonly string InsertAuth = @"
            insert into ps_auths
                (id, auth_type, username, password)
            values
                (@id, @authType::pjsip_auth_type_values_v2, @username, @password);
        ";

        private readonly string InsertEndpoint = @"
            insert into ps_endpoints
                (id, transport, aors, auth, context, disallow, allow, dtls_auto_generate_cert)
            values
                (@id, @transport, @aors, @auth, @context, @disallow, @allow, @dtlsAutoGenerateCert);
        ";

        private readonly string GetEndpointCredentials = @"
            select
                a.username,
                a.password
            from
                ps_endpoints as e
                inner join ps_auths as a
                    on e.auth = a.id
            where
                e.id = @endpoint
        ";

        /// <summary>
        /// Creates a new instance of AsteriskProvider.
        ///
        /// Exceptions
        ///     ArgumentNullException: when any parameter is null.
        /// </summary>
        public AsteriskProvider(
            ILogger<AsteriskProvider> logger,
            DatabaseOptions options
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _connection = new NpgsqlConnection(_options.ConnectionString);
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
                var parameters = new
                {
                    id = $"aor{endpoint}",
                    maxContacts = 5,
                    removeExisting = true,
                    supportPath = true
                };

                await _connection.ExecuteAsync(InsertAor, parameters);

                return new Success<Unit, Error>(Unit.New());
            } catch (Exception e)
            {
                _logger.LogError(e.Message);
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
                var parameters = new
                {
                    id = $"auth{endpoint}",
                    authType = "userpass",
                    username = endpoint.ToString(),
                    password = password.ToString()
                };

                await _connection.ExecuteAsync(InsertAuth, parameters);

                return new Success<Unit, Error>(Unit.New());
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
                var parameters = new
                {
                    id = endpoint.ToString(),
                    transport = "transport-ws",
                    aors = $"aor{endpoint}",
                    auth = $"auth{endpoint}",
                    context = "burst",
                    disallow = "all",
                    allow = "opus",
                    dtlsAutoGenerateCert = true
                };

                await _connection.ExecuteAsync(InsertEndpoint, parameters);

                return new Success<Unit, Error>(Unit.New());
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
                var info = await _connection
                    .QueryFirstAsync<AsteriskEndpoint>(GetEndpointCredentials, new { endpoint = endpoint.ToString() });

                return new Success<AsteriskEndpoint, Error>(info);
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
            var authMonad = await PostAuthAsync(endpoint, password);
            var aorMonad = await authMonad.BindAsync(_ => PostAorAsync(endpoint));
            var endpointMonad = await authMonad.BindAsync(_ => PostEndpointAsync(endpoint));

            return endpointMonad.Attach(_ => new AsteriskEndpoint
            {
                Username = endpoint.ToString(),
                Password = password.ToString()
            });
        }

        /// <summary>
        /// Executes any neccessary code for the disposal of the service instance.
        /// </summary>
        public void Dispose()
        {
            if (_connection is { })
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
