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

            object[]? parameters = null;
            if (exception is BaseBusinessException businessEx)
            {
                parameters = businessEx.Parameters;
            }

            var (statusCode, errorCode) = exception switch
            {
                FluentValidation.ValidationException valEx =>
                  (HttpStatusCode.BadRequest, string.Join(" | ", valEx.Errors.Select(e => $"{e.PropertyName}:{e.ErrorCode}"))),

                ArgumentException or ArgumentNullException or ArgumentOutOfRangeException =>
                  (HttpStatusCode.BadRequest, exception.Message),

                CustomerNotFoundException => (HttpStatusCode.NotFound, exception.Message),

                WalletNotFoundException => (HttpStatusCode.NotFound, exception.Message),

                InvalidIbanException => (HttpStatusCode.BadRequest, exception.Message),

                InsufficientBalanceException => (HttpStatusCode.BadRequest, exception.Message),

                WalletBalanceIsNotEmptyExcepiton => (HttpStatusCode.BadRequest, exception.Message),

                SameCurrencyTradeException => (HttpStatusCode.UnprocessableEntity, exception.Message),

                WalletCurrencyMismatchException => (HttpStatusCode.UnprocessableEntity, exception.Message),

                InvalidWalletTypeForTradeException => (HttpStatusCode.UnprocessableEntity, exception.Message),

                ReferenceAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),

                ConcurrencyException => (HttpStatusCode.Conflict, exception.Message),

                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message),

                BaseBusinessException => (HttpStatusCode.BadRequest, exception.Message),

                _ => (HttpStatusCode.InternalServerError, "ERR_INTERNAL_SERVER_ERROR")
            };
        

            context.Response.StatusCode = (int)statusCode;

            if (statusCode == HttpStatusCode.InternalServerError)
                Log.Error(exception, "Kritik Sistem Hatası: {Message}", exception.Message);
            else
                //   Log.Warning("İş Mantığı İhlali [Key: {ErrorCode}]: {OriginalMessage}", errorCode, exception.Message);
                Log.Warning(exception, "İş Mantığı İhlali [Key: {ErrorCode}]", errorCode);

            var response = new
            {
                Status = context.Response.StatusCode, 
                Message = errorCode,                   
                Parameters = parameters,
                Detail = exception.GetType().Name,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
