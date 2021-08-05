using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;
using Thankifi.Common.Pagination;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Category.Queries;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Category.Query
{
    public class RetrieveAllHandler : IQueryHandler<RetrieveAll, PaginatedList<CategoryDto>>
    {
        private readonly ThankifiDbContext _dbContext;

        public RetrieveAllHandler(ThankifiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedList<CategoryDto>> Handle(RetrieveAll request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Categories.AsNoTracking();
            
            var count = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(category => new CategoryDto
                {
                    Id = category.Id,
                    Slug = category.Slug
                }).ToListAsync(cancellationToken);
            
            return new PaginatedList<CategoryDto>(items, count, request.PageNumber, request.PageSize);
        }
    }
}