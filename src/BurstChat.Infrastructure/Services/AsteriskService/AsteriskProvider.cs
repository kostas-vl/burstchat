using System;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Models;
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
            (id, transport, aors, auth, context, disallow, allow, dtls_auto_generate_cert, webrtc)
        values
            (@id, @transport, @aors, @auth, @context, @disallow, @allow, @dtlsAutoGenerateCert, @webrtc);
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

    public AsteriskProvider(
        ILogger<AsteriskProvider> logger,
        DatabaseOptions options
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _connection = new NpgsqlConnection(_options.ConnectionString);
    }

    private async Task<Either<Unit, Error>> PostAorAsync(string endpoint)
    {
        try
        {
            var parameters = new
            {
                id = endpoint,
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

    private async Task<Either<Unit, Error>> PostAuthAsync(string endpoint, Guid password)
    {
        try
        {
            var parameters = new
            {
                id = $"auth{endpoint}",
                authType = "userpass",
                username = endpoint,
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

    private async Task<Either<Unit, Error>> PostEndpointAsync(string endpoint)
    {
        try
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

            return new Success<Unit, Error>(Unit.New());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new Failure<Unit, Error>(SystemErrors.Exception());
        }
    }

    public async Task<Either<AsteriskEndpoint, Error>> GetAsync(string endpoint)
    {
        try
        {
            var info = await _connection
                .QueryFirstAsync<AsteriskEndpoint>(GetEndpointCredentials, new { endpoint });

            return new Success<AsteriskEndpoint, Error>(info);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new Failure<AsteriskEndpoint, Error>(SystemErrors.Exception());
        }
    }

    public async Task<Either<AsteriskEndpoint, Error>> PostAsync(string endpoint, Guid password)
    {
        var authMonad = await PostAuthAsync(endpoint, password);
        var aorMonad = await authMonad.BindAsync(_ => PostAorAsync(endpoint));
        var endpointMonad = await authMonad.BindAsync(_ => PostEndpointAsync(endpoint));

        return endpointMonad.Attach(_ => new AsteriskEndpoint
        {
            Username = endpoint,
            Password = password.ToString()
        });
    }

    public void Dispose()
    {
        if (_connection is { })
        {
            _connection.Dispose();
            _connection = null;
        }
    }
}
