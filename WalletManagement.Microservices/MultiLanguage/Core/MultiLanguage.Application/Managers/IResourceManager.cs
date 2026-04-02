using MultiLanguage.Application.Dtos;

namespace MultiLanguage.Application.Managers
{
    public interface IResourceManager
    {
        Task<string> GetStringAsync(string key, string cultureCode);
        Task<Dictionary<string, string>> GetResourceBundleAsync(string cultureCode);
        Task CreateResourceAsync(CreateResourceDto dto);
    }
}
