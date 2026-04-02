
namespace MultiLanguage.Client.Abstracts
{
    public interface ILanguageClient
    {
         public Task<string> GetTranslationAsync(string key, string cultureCode);
    }
}

