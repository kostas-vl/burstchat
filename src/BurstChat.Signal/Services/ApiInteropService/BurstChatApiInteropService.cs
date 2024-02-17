using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Signal.Extensions;
using BurstChat.Signal.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BurstChat.Signal.Services.ApiInteropService;

public class BurstChatApiInteropService
{
    private readonly ApiDomainOptions _acceptedDomainsOptions;
    private readonly HttpClient _httpClient;

    public BurstChatApiInteropService(
        IOptions<ApiDomainOptions> acceptedDomainsOptions,
        HttpClient httpClient
    )
    {
        _acceptedDomainsOptions = acceptedDomainsOptions?.Value ?? throw new ArgumentNullException(nameof(acceptedDomainsOptions));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = new Uri(_acceptedDomainsOptions.BurstChatApiDomain);
    }

    public async Task<Either<T, Error>> SendAsync<T>(HttpContext context, HttpMethod method, string path, HttpContent? content = null)
    {
        var accessToken = context.GetAccessToken();
        var request = new HttpRequestMessage(method, path);
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        return await response.ParseBurstChatApiResponseAsync<T>();
    }

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
