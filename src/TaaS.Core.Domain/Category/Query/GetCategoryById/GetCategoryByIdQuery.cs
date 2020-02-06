using MediatR;
using TaaS.Core.Domain.Category.Dto;

namespace TaaS.Core.Domain.Category.Query.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<CategoryDetailDto?>
    {
        public int Id { get; set; }
        public string? Language { get; set; }
    }
}