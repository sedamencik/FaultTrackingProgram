using System.Net;
using System.Text.Json;
using Core.Entities; // ErrorResult'ın olduğu yer

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Bir hata oluştu: {ex.Message}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        // Hata tipine göre durum kodunu belirle
        context.Response.StatusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest, // 400
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401
            KeyNotFoundException => (int)HttpStatusCode.NotFound, // 404
            _ => (int)HttpStatusCode.InternalServerError // 500 (Geri kalan her şey)
        };

        // Senin standart ErrorResult formatın
        var response = new ErrorResult
        {
            Message = exception.Message // Sadece mesajı dönüyoruz, StackTrace gizli!
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }
}