using AutoMapper;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Core.Domain.Contract.Language.Dto;

namespace Thankifi.Api.Mapping.V1
{
    public class LanguageMappingProfile : Profile
    {
        public LanguageMappingProfile()
        {
            CreateMap<LanguageDto, LanguageViewModel>();
            CreateMap<LanguageDetailDto, LanguageDetailViewModel>();
        }
    }
}