using AutoMapper;
using MultiLanguage.Application.Dtos;
using MultiLanguage.Application.Managers;
using MultiLanguage.Application.Services;
using MultiLanguage.Contract.Repositories;
using MultiLanguage.Domain.Entities;

namespace MultiLanguage.InnerInfra.Managers
{
    public class ResourceManager : IResourceManager
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public ResourceManager(IResourceRepository resourceRepository, ILanguageRepository languageRepository, IMapper mapper, ICacheService cacheService)
        {
            _resourceRepository = resourceRepository;
            _languageRepository = languageRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task CreateResourceAsync(CreateResourceDto newResourceDto)
        {
            var entity = _mapper.Map<Resource>(newResourceDto);
            await _resourceRepository.AddResourceAsync(entity);
            await _resourceRepository.SaveChangesAsync();

            var language = await _languageRepository.GetByIdAsync(newResourceDto.LanguageId);

            if (language != null)
            {
                string cacheKey = $"resource:{language.CultureCode}:{newResourceDto.Key}";
                await _cacheService.RemoveAsync(cacheKey);
            }
        }

        public async Task<Dictionary<string, string>> GetResourceBundleAsync(string cultureCode)
        {
            return await _resourceRepository.GetAllByCultureCodeAsync(cultureCode);
        }

        public async Task<string> GetStringAsync(string key, string cultureCode)
        {
            string cacheKey = $"resource:{cultureCode}:{key}";

            var cachedValue = await _cacheService.GetAsync<string>(cacheKey);
            if (!string.IsNullOrEmpty(cachedValue)) return cachedValue;

            var value = await _resourceRepository.GetValueAsync(key, cultureCode);

            if (!string.IsNullOrEmpty(value))
            {
                await _cacheService.SetAsync(cacheKey, value, TimeSpan.FromDays(1));
            }

            return value;
        }
    }

}
