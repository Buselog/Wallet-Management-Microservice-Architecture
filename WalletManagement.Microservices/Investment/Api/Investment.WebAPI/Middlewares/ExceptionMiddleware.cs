using Investment.Domain.Exceptions;
using Serilog;
using System.Net;
using System.Text.Json;

namespace Investment.WebAPI.Middlewares
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

                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "ERR_UNAUTHORIZED"),

                BaseBusinessException => (HttpStatusCode.BadRequest, exception.Message),

                _ => (HttpStatusCode.InternalServerError, "ERR_INTERNAL_SERVER_ERROR")
            };


            context.Response.StatusCode = (int)statusCode;

            if (statusCode == HttpStatusCode.InternalServerError)
                Log.Error(exception, "Investment API - Kritik Sistem Hatası: {Message}", exception.Message);
            else
                Log.Warning("Investment API - İş Mantığı İhlali: {Message}", exception.Message);

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
