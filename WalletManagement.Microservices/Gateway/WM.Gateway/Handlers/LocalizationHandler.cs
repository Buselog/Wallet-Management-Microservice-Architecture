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

                if (errorData != null && errorData.ContainsKey("message"))
                {
                    var errorKey = errorData["message"]?.ToString();

                    if (!string.IsNullOrEmpty(errorKey) && errorKey.StartsWith("ERR_"))
                    {
                        var culture = request.Headers.AcceptLanguage.FirstOrDefault()?.Value ?? "tr-TR";

                        var translatedMessage = await GetTranslationAsync(errorKey, culture);

                        errorData["message"] = translatedMessage;

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
                var keys = key.Split('|'); 
                var translatedParts = new List<string>();

                var client = _httpClientFactory.CreateClient("MultiLanguageAPI");
                client.DefaultRequestHeaders.AcceptLanguage.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));

                foreach (var k in keys)
                {
                    var trimmedKey = k.Trim();
                    var apiResponse = await client.GetAsync($"api/resource/get-translation?key={trimmedKey}");

                    if (apiResponse.IsSuccessStatusCode)
                        translatedParts.Add(await apiResponse.Content.ReadAsStringAsync());
                    else
                        translatedParts.Add(trimmedKey); 
                }

                return string.Join(" | ", translatedParts);
            }
            catch
            {
                return key;
            }
        }
    }
}
