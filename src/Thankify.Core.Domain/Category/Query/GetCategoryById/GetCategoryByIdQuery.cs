using MediatR;
using Thankify.Core.Domain.Category.Dto;

namespace Thankify.Core.Domain.Category.Query.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<CategoryDetailDto?>
    {
        public int Id { get; set; }
        public string? Language { get; set; }
    }
}