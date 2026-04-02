using AutoMapper;
using MultiLanguage.Application.Dtos;
using MultiLanguage.Application.Managers;
using MultiLanguage.Contract.Repositories;
using MultiLanguage.Domain.Entities;

namespace MultiLanguage.InnerInfra.Managers
{
    public class LanguageManager : ILanguageManager
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;


        public LanguageManager(ILanguageRepository languageRepository, IMapper mapper)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
        }

        public async Task<List<LanguageDto>> GetAllAsync()
        {
           var languages =  await _languageRepository.GetAllLanguagesAsync();
            return _mapper.Map<List<LanguageDto>>(languages);
        }

        public async Task<LanguageDto> GetLanguageByCultureAsync(string culture)
        {
            var language = await _languageRepository.GetByCultureCodeAsync(culture);
            return _mapper.Map<LanguageDto>(language);
        }

        public async Task CreateLanguageAsync(CreateLanguageDto languageDto)
        {
            var newLanguageEntity = _mapper.Map<Language>(languageDto);
            await _languageRepository.AddLanguageAsync(newLanguageEntity);

            await _languageRepository.SaveChangesAsync();
        }

    }
}
