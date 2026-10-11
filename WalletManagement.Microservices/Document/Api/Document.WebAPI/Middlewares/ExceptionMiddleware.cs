using Document.Application.Exceptions;
using Serilog;
using System.Net;
using System.Text.Json;

namespace Document.Api.Middlewares;

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
        else if (exception is WalletServiceException walletServiceEx)
        {
            parameters = walletServiceEx.Parameters;
        }

        var (statusCode, errorCode) = exception switch
        {
            FileEmptyException => (HttpStatusCode.BadRequest, exception.Message),
            InvalidFileExtensionException => (HttpStatusCode.BadRequest, exception.Message),
            FileSizeExceededException => (HttpStatusCode.BadRequest, exception.Message),

            DocumentNotRecognizedException => (HttpStatusCode.UnprocessableEntity, exception.Message),
            InvoiceAmountNotFoundException => (HttpStatusCode.UnprocessableEntity, exception.Message),

            OcrProviderException => (HttpStatusCode.BadGateway, exception.Message),

            WalletServiceException walletEx => ((HttpStatusCode)walletEx.StatusCode, walletEx.Message),

            BaseBusinessException => (HttpStatusCode.BadRequest, exception.Message),

            _ => (HttpStatusCode.InternalServerError, "ERR_INTERNAL_SERVER_ERROR")
        };

        context.Response.StatusCode = (int)statusCode;

        if (statusCode == HttpStatusCode.InternalServerError)
            Log.Error(exception, "Document API - Kritik Sistem Hatası: {Message}", exception.Message);
        else
            Log.Warning(exception, "İş Mantığı İhlali [Key: {ErrorCode}]", errorCode);

        var response = new
        {
            Status = context.Response.StatusCode,
            Message = errorCode,
            Parameters = parameters,
            Detail = statusCode == HttpStatusCode.InternalServerError ? exception.GetType().Name : exception.Message,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}