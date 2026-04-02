using AutoMapper;
using MultiLanguage.Application.Dtos;
using MultiLanguage.Application.Managers;
using MultiLanguage.Contract.Repositories;
using MultiLanguage.Domain.Entities;

namespace MultiLanguage.InnerInfra.Managers
{
    public class ResourceManager : IResourceManager
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly IMapper _mapper;

        public ResourceManager(IResourceRepository resourceRepository, IMapper mapper)
        {
            _resourceRepository = resourceRepository;
            _mapper = mapper;
        }

        public async Task CreateResourceAsync(CreateResourceDto newResourceDto)
        {
            var entity = _mapper.Map<Resource>(newResourceDto);
            await _resourceRepository.AddResourceAsync(entity);
            await _resourceRepository.SaveChangesAsync();
        }

        public async Task<Dictionary<string, string>> GetResourceBundleAsync(string cultureCode)
        {
            return await _resourceRepository.GetAllByCultureCodeAsync(cultureCode);
        }

        public async Task<string> GetStringAsync(string key, string cultureCode)
        {
           return await _resourceRepository.GetValueAsync(key, cultureCode);
        }
    }
}
