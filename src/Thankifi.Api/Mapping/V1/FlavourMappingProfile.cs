using AutoMapper;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;

namespace Thankifi.Api.Mapping.V1;

public class FlavourMappingProfile : Profile
{
    public FlavourMappingProfile()
    {
        CreateMap<FlavourDto, FlavourViewModel>();
    }
}