using Serilog;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WM.Gateway.Handlers
{
    public class LocalizationHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LocalizationHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode && response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(content, options);

                object[] parameters = null;

                var parametersProperty = errorData.Keys.FirstOrDefault(k => k.Equals("parameters", StringComparison.OrdinalIgnoreCase));

                if (parametersProperty != null)
                {
                    var rawParams = errorData[parametersProperty];

                    Console.WriteLine($"Parametre bulundu: {rawParams}");

                    if (rawParams is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        parameters = JsonSerializer.Deserialize<object[]>(element.GetRawText());
                    }
                }

                string targetKey = errorData.ContainsKey("Message") ? "Message" : (errorData.ContainsKey("message") ? "message" : null);

                if (targetKey != null)
                {
                    var errorKey = errorData[targetKey]?.ToString();

                    if (!string.IsNullOrEmpty(errorKey) && errorKey.StartsWith("ERR_"))
                    {
                        var culture = request.Headers.AcceptLanguage.FirstOrDefault()?.Value ?? "tr-TR";
                        var translatedMessage = await GetTranslationAsync(errorKey, culture);

                        if (parameters != null && parameters.Length > 0)
                        {
                            try
                            {
                                translatedMessage = string.Format(translatedMessage, parameters);
                            }
                            catch (FormatException)
                            {
                                Log.Warning("Mesaj formatlanamadı. Şablon: {Template}", translatedMessage);
                            }
                        }

                        errorData[targetKey] = translatedMessage;

                        errorData.Remove("parameters");
                        errorData.Remove("Parameters");

                        var newContent = JsonSerializer.Serialize(errorData);
                        response.Content = new StringContent(newContent, System.Text.Encoding.UTF8, "application/json");
                    }
                }
            }
            return response;
        }

        private async Task<string> GetTranslationAsync(string key, string culture)
        {
            try
            {
                var parts = key.Split('|');
                var translatedParts = new List<string>();

                var client = _httpClientFactory.CreateClient("MultiLanguageAPI");
                client.DefaultRequestHeaders.AcceptLanguage.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));

                foreach (var p in parts)
                {
                    var trimmedPart = p.Trim();

                    if (trimmedPart.StartsWith("ERR_"))
                    {
                        var apiResponse = await client.GetAsync($"api/Resource/get-translation?key={trimmedPart}");

                        if (apiResponse.IsSuccessStatusCode)
                        {
                            var jsonResponse = await apiResponse.Content.ReadAsStringAsync();

                            using var doc = JsonDocument.Parse(jsonResponse);

                            if (doc.RootElement.TryGetProperty("message", out var messageElement))
                            {
                                translatedParts.Add(messageElement.GetString() ?? trimmedPart);
                                continue;
                            }
                        }
                        translatedParts.Add(trimmedPart);
                    
                    }

                }

                return string.Join(" | ", translatedParts);
            }
            catch (Exception ex)
            {
                return key.Split('|')[0].Trim();
            }
        }
    }
}
