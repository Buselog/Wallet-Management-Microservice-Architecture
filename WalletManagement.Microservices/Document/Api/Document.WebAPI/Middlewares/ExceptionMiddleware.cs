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

        var (statusCode, errorCode) = exception switch
        {
            // 1. Dosya Validasyon Hataları (İstemci kaynaklı geçersiz istekler -> 400 Bad Request)
            FileEmptyException => (HttpStatusCode.BadRequest, exception.Message),
            InvalidFileExtensionException => (HttpStatusCode.BadRequest, exception.Message),
            FileSizeExceededException => (HttpStatusCode.BadRequest, exception.Message),

            // 2. Belge ve Veri Çıkarım Hataları (Format doğru ama fatura okunamadı -> 422 Unprocessable Entity)
            DocumentNotRecognizedException => (HttpStatusCode.UnprocessableEntity, exception.Message),
            InvoiceAmountNotFoundException => (HttpStatusCode.UnprocessableEntity, exception.Message),

            // 3. Harici Servis / Sağlayıcı Hataları (OCR servisi çöktüyse -> 502 Bad Gateway)
            OcrProviderException => (HttpStatusCode.BadGateway, exception.Message),

            // 4. Mükerrer Kayıt (Aynı fatura daha önce ödenmişse -> 409 Conflict)
            InvoiceAlreadyProcessedException => (HttpStatusCode.Conflict, exception.Message),

            // 5. Cüzdan Servisinden Dönen Hatalar (Dış servisin HTTP kodu ve mesajı aynen korunur)
            WalletServiceException walletEx => ((HttpStatusCode)walletEx.StatusCode, walletEx.Message),

            // 6. Tanımlanmış Diğer Temel İş Kuralı Hataları
            BaseBusinessException => (HttpStatusCode.BadRequest, exception.Message),

            // 7. Öngörülemeyen Sistem Hataları
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
            Detail = exception.GetType().Name,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}