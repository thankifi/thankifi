using System.Collections.Generic;
using MediatR;
using Thankifi.Core.Domain.Category.Dto;

namespace Thankifi.Core.Domain.Category.Query.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
    {
        public string? Language { get; set; }
    }
}