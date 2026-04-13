using Polly.CircuitBreaker;
using Serilog; 
using System.Net;

namespace WM.Gateway.Handlers
{
    public class ResilienceHandler : DelegatingHandler
    {

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    Log.Warning("Resilience: Servis ulaşılamaz durumda. URL: {Url}, Status: {Status}",
                        request.RequestUri, response.StatusCode);

                    return CreateCustomErrorResponse(HttpStatusCode.ServiceUnavailable, "ERR_SERVICE_UNREACHABLE");
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "Resilience: Servis tamamen OFFLINE. URL: {Url}", request.RequestUri);
                return CreateCustomErrorResponse(HttpStatusCode.ServiceUnavailable, "ERR_SERVICE_OFFLINE");
            }
            catch (TaskCanceledException ex)
            {
                Log.Error(ex, "Resilience: Servis zaman aşımına uğradı (Timeout). URL: {Url}", request.RequestUri);
                return CreateCustomErrorResponse(HttpStatusCode.GatewayTimeout, "ERR_SERVICE_TIMEOUT");
            }
            catch (BrokenCircuitException ex)
            {
                Log.Warning(ex, "Resilience: Polly, servisi bir süreliğine devre dışı bıraktı. Servis karantinada.");
                return CreateCustomErrorResponse(HttpStatusCode.ServiceUnavailable, "ERR_SERVICE_QUARANTINED");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Resilience: Gateway katmanında beklenmedik kritik hata.");
                return CreateCustomErrorResponse(HttpStatusCode.InternalServerError, "ERR_GATEWAY_RESILIENCE_ERROR");
            }
        }

        private HttpResponseMessage CreateCustomErrorResponse(HttpStatusCode statusCode, string errorCode)
        {
            var errorResponse = new
            {
                Status = (int)statusCode,
                Message = errorCode,
                Detail = "Gateway Resilience System",
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(errorResponse),
                          System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}