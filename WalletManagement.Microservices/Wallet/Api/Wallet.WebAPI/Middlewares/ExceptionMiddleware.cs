using Serilog;
using System.Net;
using System.Text.Json;
using Wallet.Domain.Exceptions;

namespace Wallet.WebAPI.Middlewares
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

                InvalidIbanException => (HttpStatusCode.BadRequest, exception.Message),

                InsufficientBalanceException => (HttpStatusCode.BadRequest, exception.Message),

                ReferenceAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),

                ConcurrencyException => (HttpStatusCode.Conflict, exception.Message),

                BaseBusinessException => (HttpStatusCode.BadRequest, exception.Message),

                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Bu işlem için yetkiniz bulunmamaktadır."),

                _ => (HttpStatusCode.InternalServerError, "Sunucu taraflı beklenmedik bir hata oluştu. Lütfen daha sonra tekrar deneyin.")
            };
        

            context.Response.StatusCode = (int)statusCode;

            if (statusCode == HttpStatusCode.InternalServerError)
                Log.Error(exception, "Kritik Sistem Hatası: {Message}", exception.Message);
            else
                Log.Warning("İş Mantığı İhlali: {Message}", exception.Message);

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
