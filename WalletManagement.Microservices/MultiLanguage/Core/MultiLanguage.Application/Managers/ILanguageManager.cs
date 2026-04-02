using MultiLanguage.Application.Dtos;

namespace MultiLanguage.Application.Managers
{
    public interface ILanguageManager
    {
        Task<List<LanguageDto>> GetAllAsync();
        Task<LanguageDto> GetLanguageByCultureAsync(string culture);
        Task CreateLanguageAsync(CreateLanguageDto dto);
    }
}


