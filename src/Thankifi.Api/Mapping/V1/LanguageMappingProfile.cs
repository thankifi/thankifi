using AutoMapper;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Core.Domain.Contract.Language.Dto;

namespace Thankifi.Api.Mapping.V1;

public class LanguageMappingProfile : Profile
{
    public LanguageMappingProfile()
    {
        CreateMap<LanguageDto, LanguageViewModel>()
            .ForMember(d => d.Reference, opt => opt.MapFrom(s => $"https://iso639-3.sil.org/code/{s.Code}"));
        CreateMap<LanguageDetailDto, LanguageDetailViewModel>()
            .ForMember(d => d.Reference, opt => opt.MapFrom(s => $"https://iso639-3.sil.org/code/{s.Code}"));
    }
}