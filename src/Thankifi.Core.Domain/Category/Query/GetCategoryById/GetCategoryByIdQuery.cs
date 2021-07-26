using MediatR;
using Thankifi.Core.Domain.Category.Dto;

namespace Thankifi.Core.Domain.Category.Query.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<CategoryDetailDto?>
    {
        public int Id { get; set; }
        public string? Language { get; set; }
    }
}