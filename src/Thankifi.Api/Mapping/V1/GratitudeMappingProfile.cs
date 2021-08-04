using AutoMapper;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;

namespace Thankifi.Api.Mapping.V1
{
    public class GratitudeMappingProfile : Profile
    {
        public GratitudeMappingProfile()
        {
            CreateMap<GratitudeDto, GratitudeViewModel>();
            CreateMap<GratitudeFlavourfulDto, GratitudeFlavourfulViewModel>();
        }
    }
}