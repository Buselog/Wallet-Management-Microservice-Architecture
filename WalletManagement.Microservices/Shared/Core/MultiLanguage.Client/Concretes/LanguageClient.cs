using MultiLanguage.Client.Abstracts;
using System.Net.Http.Headers;

namespace MultiLanguage.Client.Concretes
{
    public class LanguageClient : ILanguageClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public LanguageClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetTranslationAsync(string key, string cultureCode)
        {
            try
            {
                var client = _clientFactory.CreateClient("MultiLanguageAPI");

                client.DefaultRequestHeaders.AcceptLanguage.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureCode));

                var response = await client.GetAsync($"api/resource/get-translation?key={key}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                return key;

            }
            catch (Exception)
            {
                return key;
            }
        }
    }
}
