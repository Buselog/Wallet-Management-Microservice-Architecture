using MultiLanguage.Domain.Entities;

namespace MultiLanguage.Contract.Repositories
{
    public interface ILanguageRepository
    {
        Task<List<Language>> GetAllLanguagesAsync();
        Task<Language> GetByCultureCodeAsync(string cultureCode);
        Task AddLanguageAsync(Language language);
        Task<int> SaveChangesAsync();
        Task<Language> GetByIdAsync(int id);
    }
}

