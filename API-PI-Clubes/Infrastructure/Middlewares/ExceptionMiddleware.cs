using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using System.Text.Json;

namespace API_PI_Clubes.Infrastructure.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Erro de aplicação: {ErrorCode}", ex.ErrorCode);
                await WriteError(context, ex.StatusCode, ex.ErrorCode, ex.Message, 
                    ex is ValidationException v ? v.Errors : null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado");
                await WriteError(context, 500, "INTERNAL_ERROR", 
                    "Ocorreu um erro inesperado. Tente novamente mais tarde.");
            }
        }

        private static Task WriteError(HttpContext context, int statusCode, string errorCode, 
            string message, IDictionary<string, string[]>? errors = null)
        {
            var response = new ErrorResponseDTO
            {
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Message = message,
                Errors = errors,
                TraceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}