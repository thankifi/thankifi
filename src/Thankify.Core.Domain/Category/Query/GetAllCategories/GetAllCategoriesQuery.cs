using System.Collections.Generic;
using MediatR;
using Thankify.Core.Domain.Category.Dto;

namespace Thankify.Core.Domain.Category.Query.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
    {
        public string? Language { get; set; }
    }
}