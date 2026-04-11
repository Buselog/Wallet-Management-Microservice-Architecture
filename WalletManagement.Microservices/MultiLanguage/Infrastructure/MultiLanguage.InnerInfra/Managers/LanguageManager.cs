using AutoMapper;
using MultiLanguage.Application.Dtos;
using MultiLanguage.Application.Managers;
using MultiLanguage.Application.Services;
using MultiLanguage.Contract.Repositories;
using MultiLanguage.Domain.Entities;

namespace MultiLanguage.InnerInfra.Managers
{
    public class LanguageManager : ILanguageManager
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;


        public LanguageManager(ILanguageRepository languageRepository, IMapper mapper, ICacheService cacheService)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<List<LanguageDto>> GetAllAsync()
        {
            string cacheKey = "lang:all";

            var cachedLanguages = await _cacheService.GetAsync<List<LanguageDto>>(cacheKey);
            if (cachedLanguages != null) return cachedLanguages;

            var languages =  await _languageRepository.GetAllLanguagesAsync();
            var languageDtos = _mapper.Map<List<LanguageDto>>(languages);

            if (languageDtos!= null && languageDtos.Any())
            {
                await _cacheService.SetAsync(cacheKey, languageDtos, TimeSpan.FromDays(1));
            }

            return languageDtos;

        }

        public async Task<LanguageDto> GetLanguageByCultureAsync(string culture)
        {
            string cacheKey = $"lang:{culture}";

            var cachedLang = await _cacheService.GetAsync<LanguageDto>(cacheKey);
            if (cachedLang != null) return cachedLang;

            var language = await _languageRepository.GetByCultureCodeAsync(culture);
            var languageDto = _mapper.Map<LanguageDto>(language);

            if (languageDto != null)
            {
                await _cacheService.SetAsync(cacheKey, languageDto, TimeSpan.FromDays(1)); 
            }

            return languageDto;
        }

        public async Task CreateLanguageAsync(CreateLanguageDto languageDto)
        {
            var newLanguageEntity = _mapper.Map<Language>(languageDto);
            await _languageRepository.AddLanguageAsync(newLanguageEntity);

            await _languageRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"lang:{languageDto.CultureCode}");
            await _cacheService.RemoveAsync("lang:all");
        }

    }
}
