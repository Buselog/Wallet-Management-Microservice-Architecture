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

            var (statusCode, errorCode) = exception switch
            {
                FluentValidation.ValidationException valEx =>
                 (HttpStatusCode.BadRequest, string.Join(" | ", valEx.Errors.Select(e => $"{e.PropertyName}:{e.ErrorCode}"))),

                ArgumentNullException or ArgumentException or ArgumentOutOfRangeException =>
                 (HttpStatusCode.BadRequest, exception.Message),

                CustomerNotFoundException => (HttpStatusCode.NotFound, exception.Message),

                EmailAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),

                InvalidCredentialsException => (HttpStatusCode.Unauthorized, exception.Message),

                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "ERR_UNAUTHORIZED"),

                BaseBusinessException => (HttpStatusCode.BadRequest, exception.Message),

                _ => (HttpStatusCode.InternalServerError, "ERR_INTERNAL_SERVER_ERROR")
            };

            context.Response.StatusCode = (int)statusCode;

            if (statusCode == HttpStatusCode.InternalServerError)
                Log.Error(exception, "Customer API - Kritik Sistem Hatası: {Message}", exception.Message);
            else
                Log.Warning(exception, "İş Mantığı İhlali [Key: {ErrorCode}]", errorCode);
           
            var response = new
            {
                Status = context.Response.StatusCode,
                Message = errorCode,
                Detail = exception.GetType().Name,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

