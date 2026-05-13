using System.Net;
using System.Text.Json;

namespace RentACar.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate            _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment           _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next   = next;
            _logger = logger;
            _env    = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;

            // Always include the full message so you can diagnose production 500s.
            // Once the app is stable you can remove InnerException from the response.
            var response = new
            {
                StatusCode     = context.Response.StatusCode,
                Message        = ex.Message,
                InnerException = ex.InnerException?.Message,
                Type           = ex.GetType().Name
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}