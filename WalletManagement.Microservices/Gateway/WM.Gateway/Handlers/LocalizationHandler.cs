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

            bool isPreviewRoute = request.RequestUri.AbsolutePath.Contains("preview", StringComparison.OrdinalIgnoreCase);

            if (response.IsSuccessStatusCode && !isPreviewRoute)
            {
                return response;
            }

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(content, options);

                if (errorData == null) return response;

                object[] parameters = null;
                var parametersProperty = errorData.Keys.FirstOrDefault(k =>
                    k.Equals("parameters", StringComparison.OrdinalIgnoreCase) ||
                    k.Equals("validationparameters", StringComparison.OrdinalIgnoreCase));

                if (parametersProperty != null)
                {
                    var rawParams = errorData[parametersProperty];
                    if (rawParams is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        parameters = JsonSerializer.Deserialize<object[]>(element.GetRawText());
                    }
                }

                string targetKey = null;
                if (errorData.ContainsKey("ValidationMessage") || errorData.ContainsKey("validationMessage"))
                {
                    targetKey = errorData.Keys.First(k => k.Equals("validationMessage", StringComparison.OrdinalIgnoreCase));
                }
                else if (errorData.ContainsKey("Message") || errorData.ContainsKey("message"))
                {
                    targetKey = errorData.Keys.First(k => k.Equals("message", StringComparison.OrdinalIgnoreCase));
                }

                if (targetKey != null)
                {
                    var errorKey = errorData[targetKey]?.ToString();

                    if (!string.IsNullOrEmpty(errorKey) && (errorKey.StartsWith("ERR_") || errorKey.Contains(":ERR_")))
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
                        errorData.Remove("validationParameters");
                        errorData.Remove("ValidationParameters");

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
                var parts = key.Split(new[] { " | " }, StringSplitOptions.RemoveEmptyEntries);
                var translatedParts = new List<string>();

                var client = _httpClientFactory.CreateClient("MultiLanguageAPI");
                client.DefaultRequestHeaders.AcceptLanguage.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));

                foreach (var p in parts)
                {
                    var trimmedPart = p.Trim();

                    if (trimmedPart.Contains(":"))
                    {
                        var subParts = trimmedPart.Split(':', 2);
                        var prefix = subParts[0].Trim();
                        var possibleKey = subParts[1].Trim();

                        if (possibleKey.StartsWith("ERR_"))
                        {
                            var apiResponse = await client.GetAsync($"api/Resource/get-translation?key={possibleKey}");
                            if (apiResponse.IsSuccessStatusCode)
                            {
                                var jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                                using var doc = JsonDocument.Parse(jsonResponse);
                                if (doc.RootElement.TryGetProperty("message", out var messageElement))
                                {
                                    translatedParts.Add($"{prefix}: {messageElement.GetString()}");
                                    continue;
                                }
                            }
                        }
                    }
                    else if (trimmedPart.StartsWith("ERR_"))
                    {
                        var apiResponse = await client.GetAsync($"api/Resource/get-translation?key={trimmedPart}");
                        if (apiResponse.IsSuccessStatusCode)
                        {
                            var jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                            using var doc = JsonDocument.Parse(jsonResponse);
                            if (doc.RootElement.TryGetProperty("message", out var messageElement))
                            {
                                translatedParts.Add(messageElement.GetString());
                                continue;
                            }
                        }
                    }

                }

                return translatedParts.Any() ? string.Join(" | ", translatedParts) : key;
            }
            catch (Exception)
            {
                return key;
            }
        }
    }
}
