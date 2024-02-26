using System;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Models;
using BurstChat.Application.Monads;
using BurstChat.Infrastructure.Options;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace BurstChat.Infrastructure.Services.AsteriskService;

public class AsteriskProvider : IAsteriskService, IDisposable
{
    private readonly ILogger<AsteriskProvider> _logger;
    private readonly DatabaseOptions _options;
    private NpgsqlConnection _connection;

    private readonly string InsertAor =
        @"
        insert into ps_aors
            (id, max_contacts, remove_existing, support_path)
        values
            (@id, @maxContacts, @removeExisting, @supportPath);
    ";

    private readonly string InsertAuth =
        @"
        insert into ps_auths
            (id, auth_type, username, password)
        values
            (@id, @authType::pjsip_auth_type_values_v2, @username, @password);
    ";

    private readonly string InsertEndpoint =
        @"
        insert into ps_endpoints
            (id, transport, aors, auth, context, disallow, allow, dtls_auto_generate_cert, webrtc)
        values
            (@id, @transport, @aors, @auth, @context, @disallow, @allow, @dtlsAutoGenerateCert, @webrtc);
    ";

    private readonly string GetEndpointCredentials =
        @"
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

    public AsteriskProvider(ILogger<AsteriskProvider> logger, DatabaseOptions options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _connection = new NpgsqlConnection(_options.ConnectionString);
    }

    private Task<Result<Unit>> PostAorAsync(string endpoint) =>
        _connection
            .MapAsync(async conn =>
            {
                var parameters = new
                {
                    id = endpoint,
                    maxContacts = 5,
                    removeExisting = true,
                    supportPath = true
                };
                await conn.ExecuteAsync(InsertAor, parameters);
                return Unit.Instance;
            })
            .InspectErrAsync(e => _logger.LogError(e.Message));

    private Task<Result<Unit>> PostAuthAsync(string endpoint, Guid password) =>
        _connection
            .MapAsync(async conn =>
            {
                var parameters = new
                {
                    id = $"auth{endpoint}",
                    authType = "userpass",
                    username = endpoint,
                    password = password.ToString()
                };
                await conn.ExecuteAsync(InsertAuth, parameters);
                return Unit.Instance;
            })
            .InspectErrAsync(e => _logger.LogError(e.Message));

    private Task<Result<Unit>> PostEndpointAsync(string endpoint) =>
        _connection
            .MapAsync(async conn =>
            {
                var parameters = new
                {
                    id = endpoint,
                    transport = "transport-ws",
                    aors = endpoint,
                    auth = $"auth{endpoint}",
                    context = "burst",
                    disallow = "all",
                    allow = "opus",
                    dtlsAutoGenerateCert = true,
                    webrtc = true
                };
                await _connection.ExecuteAsync(InsertEndpoint, parameters);
                return Unit.Instance;
            })
            .InspectErrAsync(e => _logger.LogError(e.Message));

    public Task<Result<AsteriskEndpoint>> GetAsync(string endpoint) =>
        _connection
            .MapAsync(conn =>
                conn.QueryFirstAsync<AsteriskEndpoint>(GetEndpointCredentials, new { endpoint })
            )
            .InspectErrAsync(e => _logger.LogError(e.Message));

    public Task<Result<AsteriskEndpoint>> PostAsync(string endpoint, Guid password) =>
        PostAuthAsync(endpoint, password)
            .AndAsync(_ => PostAorAsync(endpoint))
            .AndAsync(_ => PostEndpointAsync(endpoint))
            .MapAsync(_ => new AsteriskEndpoint
            {
                Username = endpoint,
                Password = password.ToString()
            });

    public void Dispose()
    {
        if (_connection is { })
        {
            _connection.Dispose();
            _connection = null;
        }
    }
}
