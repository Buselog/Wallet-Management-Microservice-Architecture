using Customer.Domain.Exceptions;
using Serilog;
using System.Net;
using System.Text.Json;

namespace Customer.WebAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                CustomerNotFoundException => (HttpStatusCode.NotFound, exception.Message),

                EmailAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),

                InvalidCredentialsException => (HttpStatusCode.Unauthorized, exception.Message),

                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Bu işlem için yetkiniz bulunmamaktadır."),

                BaseBusinessException => (HttpStatusCode.BadRequest, exception.Message),

                _ => (HttpStatusCode.InternalServerError, "Müşteri tarafında beklenmedik bir hata oluştu.")
            };

            context.Response.StatusCode = (int)statusCode;

            if (statusCode == HttpStatusCode.InternalServerError)
                Log.Error(exception, "Customer API - Kritik Sistem Hatası: {Message}", exception.Message);
            else
                Log.Warning("Customer API - İş Mantığı İhlali: {Message}", exception.Message);

            var response = new
            {
                Status = context.Response.StatusCode,
                Message = message,
                Detail = exception.GetType().Name,
                Timestamp = DateTime.UtcNow 
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

