using System.Collections.Generic;
using MediatR;
using TaaS.Core.Domain.Category.Dto;

namespace TaaS.Core.Domain.Category.Query.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
    {
        public string? Language { get; set; }
    }
}