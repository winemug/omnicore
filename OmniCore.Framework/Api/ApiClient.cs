using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using OmniCore.Services.Interfaces.Amqp;
using OmniCore.Services.Interfaces.Core;
using OmniCore.Shared.Api;

namespace OmniCore.Common.Api;

public class ApiClient : IApiClient
{
    private IAppConfiguration _appConfiguration;

    private HttpClient _httpClient;
    public ApiClient(IAppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
        _httpClient = new HttpClient();
    }
    public async Task<TResponse?> PostRequestAsync<TRequest, TResponse>(string route, TRequest request,
        CancellationToken cancellationToken = default) where TResponse : ApiResponse
    {
        try
        {
            var result = await _httpClient.PostAsJsonAsync(new Uri(route, UriKind.Relative),
                request, cancellationToken);
            return await result.Content.ReadFromJsonAsync<TResponse>((JsonSerializerOptions?)null, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return null;
        }
        
    }

    private bool _disposed;
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}