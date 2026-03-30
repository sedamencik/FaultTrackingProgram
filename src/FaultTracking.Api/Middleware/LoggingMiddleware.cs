using System.Diagnostics;

namespace Middleware;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew(); // Süreyi ölçmek için
        try
        {
            await _next(context); // İsteği bir sonraki adıma gönder
            sw.Stop();

            // Başarılı istekleri logla
            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                sw.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            // Hataları detaylı logla
            _logger.LogError(ex, 
                "HTTP {Method} {Path} FAILED with error: {Message} in {Elapsed:0.0000} ms",
                context.Request.Method,
                context.Request.Path,
                ex.Message,
                sw.Elapsed.TotalMilliseconds);
            throw; // Hatayı yönetmesi için yukarı fırlat
        }
    }
}