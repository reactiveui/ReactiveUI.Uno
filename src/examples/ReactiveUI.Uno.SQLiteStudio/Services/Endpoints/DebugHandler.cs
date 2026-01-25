namespace ReactiveUI.Uno.SQLiteStudio.Services.Endpoints;

internal partial class DebugHttpHandler : DelegatingHandler
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    public DebugHttpHandler(Microsoft.Extensions.Logging.ILogger<DebugHttpHandler> logger, HttpMessageHandler? innerHandler = null)
        : base(innerHandler ?? new HttpClientHandler())
    {
        _logger = logger;
    }

    protected async override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
#if DEBUG
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogDebug("Unsuccessful API Call");
            if (request.RequestUri is not null)
            {
                _logger.LogDebug("{RequestUri} ({Method})", request.RequestUri, request.Method);
            }

            foreach ((var key, var values) in request.Headers.ToDictionary(x => x.Key, x => string.Join(", ", x.Value)))
            {
                _logger.LogDebug("{Key}: {Values}", key, values);
            }

            var content = request.Content is not null ? await request.Content.ReadAsStringAsync(cancellationToken) : null;
            if (!string.IsNullOrEmpty(content))
            {
                _logger.LogDebug("{Content}", content);
            }
        }
#endif
        return response;
    }
}
