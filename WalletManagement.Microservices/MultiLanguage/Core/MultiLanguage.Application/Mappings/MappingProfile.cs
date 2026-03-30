using AutoMapper;
using MultiLanguage.Application.Dtos;
using MultiLanguage.Domain.Entities;

namespace MultiLanguage.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Resource, ResourceDto>()
            .ForMember(dest => dest.LanguageCode,
                       opt => opt.MapFrom(src => src.Language.CultureCode));

            CreateMap<CreateResourceDto, Resource>();

            CreateMap<CreateLanguageDto, Language>();

            CreateMap<Language, LanguageDto>();
        }
    }
}
