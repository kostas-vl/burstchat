using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BurstChat.Infrastructure.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(content);
    }
}
