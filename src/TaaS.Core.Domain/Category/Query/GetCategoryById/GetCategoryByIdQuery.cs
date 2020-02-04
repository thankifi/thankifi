using MediatR;
using TaaS.Core.Domain.Category.Dto;

namespace TaaS.Core.Domain.Category.Query.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<CategoryDetailDto?>
    {
        public GetCategoryByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}