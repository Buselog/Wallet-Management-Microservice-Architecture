using MultiLanguage.Domain.Entities;

namespace MultiLanguage.Contract.Repositories
{
    public interface IResourceRepository
    {
        Task<string> GetValueAsync(string key, string cultureCode);
        Task<Dictionary<string, string>> GetAllByCultureCodeAsync(string cultureCode);
        Task<List<Resource>> GetAllAsync();
        Task AddResourceAsync(Resource resource);
        void UpdateResource(Resource resource);
        Task<int> SaveChangesAsync();

    }
}

