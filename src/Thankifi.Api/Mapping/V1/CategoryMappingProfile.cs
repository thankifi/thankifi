using AutoMapper;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Core.Domain.Contract.Category.Dto;

namespace Thankifi.Api.Mapping.V1;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CategoryDto, CategoryViewModel>();
        CreateMap<CategoryDetailDto, CategoryDetailViewModel>();
    }
}